using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Be.Corebvba.Aubergine.Extensions;
using Be.Corebvba.Aubergine.Interfaces;
using Be.Corebvba.Aubergine.Model;

namespace Be.Corebvba.Aubergine.Services
{
    public class ElementRunner
    {
        const BindingFlags defaultflags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public IElement RunStory(IStoryContainer storycontainer)
        {
            var element = storycontainer.Story;
            try
            {
                if (element.Type != ElementType.Story) return element;
                var result = element.Clone();
                var newchildren = new List<IElement>(result.Children.Where(o => o.Type != ElementType.Scenario));
                foreach (var sc in element.Children.Where(e => e.Type == ElementType.Scenario))
                {
                    var example = GetData(sc.Children.Where(o => o.Type == ElementType.Example).FirstOrDefault(),storycontainer.ColumnToken);
                    if (example.Keys.Count() > 0)
                    {
                         for (var j=0;j<example[example.Keys.First()].Count;j++)
                         {
                             IElement x = sc.Clone();
                             x.Children = x.Children.Where(o => o.Type != ElementType.Example).Select(o => o.Clone()).ToArray();

                            foreach (var n in example.Keys)
                            {
                                x.Description = x.Description.Replace(n, example[n][j]);
                                foreach (var y in x.Children)
                                    y.Description = y.Description.Replace(n, example[n][j]);
                            }
                            newchildren.Add(x);
                        }
                    }
                    else
                    {
                        newchildren.Add(sc);
                    }
                }
                result.Children = newchildren;
                foreach (var q in result.Children)
                    RunScenario(q, Activator.CreateInstance(storycontainer.ContextType),storycontainer.ColumnToken);
                return result;

            }
            catch (Exception ex)
            {
                element.Status = null;
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                element.StatusInfo = ex.GetType().Name + ":" + ex.Message;
            }
            return element;
        }

        private Dictionary<string, List<string>> GetData(IElement e,string columnseperator)
        {
            var l = new Dictionary<string, List<string>>();
            if (e == null) return l;
            var rows = e.Children.Where(o => o.Type == ElementType.Data).ToArray();
            if (rows.Count() < 2) return l;
            var names = rows[0].Description.Split(columnseperator.ToCharArray()).Select(o=>o.Trim()).ToArray();
            foreach (var r in rows.Skip(1))
            {
                var vals = r.Description.Split(columnseperator.ToCharArray()).Select(o=>o.Trim()).ToArray();
                for (int i = 1; i < names.Count()-1; i++)
                {
                    if (!l.ContainsKey(names[i]))
                        l.Add(names[i],new List<string>());
                    if (i >= vals.Length)
                        l[names[i]].Add("");
                    else
                        l[names[i]].Add(vals[i]);
                }
            }
            return l;
        }

        public void RunScenario(IElement element, object context,string columnseperator)
        {
            try
            {
                if (element.Type != ElementType.Scenario) return;
                foreach (var x in element.Parent.Children.Where(e => e.Type == ElementType.Given))
                    RunStep(x, context,columnseperator);
                foreach (var y in new ElementType[] { ElementType.Given, ElementType.When, ElementType.Then })
                    foreach (var x in element.Children.Where(e => e.Type == y))
                        RunStep(x, context,columnseperator);
            }
            catch (Exception ex)
            {
                element.Status = null;
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                element.StatusInfo = ex.GetType().Name + ":" + ex.Message;
            }
        }

        void RunStep(IElement element, object context,string columnseparator)
        {
            try
            {
                var res = CallContextDSL(element.Description, context, element.Type.ToString(),()=>GetData(element,columnseparator));
                if (element.Type == ElementType.Then)
                    element.Status = (bool)res;
                else
                    element.Status = true;
            }
            catch (Exception ex)
            {
                element.Status = null;
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                element.StatusInfo = ex.GetType().Name + ":" + ex.Message;
            }

        }

        public static object CallContextDSL(string name, object context, string phasename, Func<Dictionary<string, List<string>>> GetTable)
        {
            object returnresult = null;
            var tbl = GetTable();
            foreach (var mi in context.GetType().GetMethods(defaultflags))
                foreach (var attr in mi.GetCustomAttributes(typeof(DSLAttribute), true))
                {
                    var regex = ((DSLAttribute)attr).MyRegEx ?? mi.Name;
                    var match = Regex.Match(name, "^" + regex + "$", RegexOptions.IgnoreCase);
                    if (!match.Success)
                    {
                        if (regex == mi.Name)
                        {
                            regex = regex.Replace("_", " ").Trim();
                            match = Regex.Match(name, "^" + regex + "$", RegexOptions.IgnoreCase);
                        }
                    }
                    if (!match.Success) continue;
                    var pars = new List<object>();
                    foreach (var pi in mi.GetParameters())
                    {
                        var strval = match.Groups[pi.Name].Value;
                        object result = null;
                        if (tbl != null && tbl.Keys.Count > 0 && tbl.ContainsKey(pi.Name) && pi.ParameterType.IsArray)
                        {
                            var eltype = pi.ParameterType.GetElementType();
                            var arr = Array.CreateInstance(eltype,tbl[pi.Name].Count);
                            for (int i =0;i< arr.Length;i++)
                            {
                                arr.SetValue(ConvertTypeForDSL(tbl[pi.Name][i],context,eltype ),i);
                            }
                            result = arr;
                        }
                        else
                        {
                            if (strval == match.Value)
                                result = Convert.ChangeType(strval, pi.ParameterType);
                            else
                            {
                                result = ConvertTypeForDSL(strval, context, pi.ParameterType);
                            }
                        }
                        pars.Add(result);
                    }
                    try
                    {
                        returnresult = mi.Invoke(context, pars.ToArray());
                    }
                    catch (Exception exc)
                    {
                        try
                        {
                            context.Set(phasename + "Exception", exc);
                            return returnresult;
                        }
                        catch (Exception)
                        {

                        }
                        throw;
                    }
                    return returnresult;
                }
            throw new NotImplementedException(string.Format("DSL<{0}> can not be found/called:\n {1}", context.GetType(), name));
        }

        private static object ConvertTypeForDSL(string strval,object context, Type desttype)
        {
            object result = null;
            try
            {
                result = CallContextDSL(strval, context, "ghkazbnkazbkeaz", () => null);
            }
            catch (Exception)
            {
                if (desttype.IsEnum)
                    result = Enum.Parse(desttype, strval, true);
                else
                    result = Convert.ChangeType(strval, desttype);
            }
            return result;
        }

    }
}
