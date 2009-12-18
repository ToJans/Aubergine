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
    public class SpecRunner
    {
        const BindingFlags defaultflags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance|BindingFlags.Static|
            BindingFlags.GetProperty|BindingFlags.SetProperty|BindingFlags.GetField|BindingFlags.SetField;

        IDSLRunner _DslRunner;

        public IDSLDefinition GetDSL(ISpecElement story,Dictionary<string,IDSLDefinition> dsls)
        {
            var key = story.Description.Trim().ToLower();
            var n = story.Children.Where(e => e.Type == ElementType.Context).Select(e => e.Description.Trim().ToLower()).FirstOrDefault();
            if (string.IsNullOrEmpty(n))
                n = dsls.Keys.First();
            else
                foreach(var k in dsls.Keys)
                    if (k.ToLower().Trim()==n.ToLower())
                       return dsls[k];
            return null;
        }

        public ISpecElement RunStory(ISpecElement element,IDSLDefinition DSL,string columnseparator)
        {
            _DslRunner = new DSLRunner(DSL);
            try
            {
                if (element.Type != ElementType.Story) return element;
                var result = element.Clone();
                var newchildren = new List<ISpecElement>(result.Children.Where(o => o.Type != ElementType.Scenario));
                foreach (var sc in element.Children.Where(e => e.Type == ElementType.Scenario))
                {
                    var example = GetData(sc.Children.Where(o => o.Type == ElementType.Example).FirstOrDefault(), columnseparator);
                    if (example.Keys.Count() > 0)
                    {
                        for (var j = 0; j < example[example.Keys.First()].Count; j++)
                        {
                            ISpecElement x = sc.Clone();
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
                    RunScenario(q, _DslRunner.GetNewContext(), columnseparator, true);
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

        private Dictionary<string, List<string>> GetData(ISpecElement e, string columnseperator)
        {
            var l = new Dictionary<string, List<string>>();
            if (e == null) return l;
            var rows = e.Children.Where(o => o.Type == ElementType.Data).ToArray();
            if (rows.Count() < 2) return l;
            var names = rows[0].Description.ToLower().Split(columnseperator.ToCharArray()).Select(o => o.Trim()).ToArray();
            foreach (var r in rows.Skip(1))
            {
                var vals = r.Description.Split(columnseperator.ToCharArray()).Select(o => o.Trim()).ToArray();
                for (int i = 1; i < names.Count() - 1; i++)
                {
                    if (!l.ContainsKey(names[i]))
                        l.Add(names[i], new List<string>());
                    if (i >= vals.Length)
                        l[names[i]].Add("");
                    else
                        l[names[i]].Add(vals[i]);
                    r.Status = true;
                }
                rows[0].Status = true;
            }
            return l;
        }

        public void RunScenario(ISpecElement element, object context, string columnseperator, bool includeStoryGivens)
        {
            try
            {
                if (element.Type != ElementType.Scenario) return;
                if (includeStoryGivens)
                {
                    foreach (var x in element.Parent.Children.Where(e => e.Type == ElementType.Given))
                        RunStep(x, context, columnseperator);
                }
                var els = new ElementType[] { ElementType.GivenIdid, ElementType.Given, ElementType.When, ElementType.Then };
                foreach (var x in element.Children.Where(e => els.Contains(e.Type)))
                     RunStep(x, context, columnseperator);
            }
            catch (Exception ex)
            {
                element.Status = null;
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                element.StatusInfo = ex.GetType().Name + ":" + ex.Message;
            }
        }



        void RunStep(ISpecElement element, object context, string columnseparator)
        {
            try
            {
                if (element.Type == ElementType.GivenIdid)
                {
                    var scenario = element.Parent.Parent.Children
                        .Where(o => o.Type == ElementType.Scenario 
                            && string.Compare(o.Description, element.Description, true) == 0)
                        .FirstOrDefault();
                    if (scenario == null)
                        throw new ArgumentException("Unknown scenario : " + element.Description);
                    RunScenario(scenario, context, columnseparator, false);
                }
                else
                {

                    var res = _DslRunner.CallContextDSL(element.Description, context, element.Type.ToString(), () => GetData(element, columnseparator));
                    if (element.Type == ElementType.Then)
                        element.Status = (bool)res;
                    else
                        element.Status = true;
                }
            }
            catch (Exception ex)
            {
                element.Status = null;
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                element.StatusInfo = ex.GetType().Name + ":" + ex.Message;
            }

        }

    }
}
