using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Be.Corebvba.Aubergine.Runner
{
    public class RunnerImpl
    {
        public const BindingFlags defaultflags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public IEnumerable<ITestResult> RunStories(Type[] types)
        {
            foreach (var t in types)
            {
                if (t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == typeof(Story<>))
                {
                    yield return new TestResult(t.Name
                              , (tr) => tr.Children.All(itr => itr.Status == true)
                              , () => RunScenario(t)
                              );

                }
            }
        }

        public IEnumerable<ITestResult> RunScenario(Type StoryType)
        {
                foreach (var scenarios in StoryType.GetNestedTypes(defaultflags)
                                    .Where(it => it.BaseType.Name == "Scenario"))
                {
                    foreach (var scen2 in RunScenarioInternal(StoryType,scenarios))
                    {
                        yield return scen2;
                    }
                }
        }

        IEnumerable<ITestResult> RunScenarioInternal(Type StoryType, Type ScenarioType)
        {
            var colnames = new List<string>();
            var datavalues = new List<List<String>>();

            foreach (var attr in ScenarioType.GetCustomAttributes(typeof(ColsAttribute), true).Cast<ColsAttribute>())
            {
                colnames.AddRange(attr.Names);
            }

            foreach (var attr in ScenarioType.GetCustomAttributes(typeof(DataAttribute), true).Cast<DataAttribute>())
            {
                if (attr.Vals.Length != colnames.Count)
                    throw new ArgumentException("Wrong number of data values");
                var l = new List<string>();
                l.AddRange(attr.Vals.Select(o => o.ToString()));
                datavalues.Add(l);
            }

            if (colnames.Count > 1 && datavalues.Count == 0)
                throw new ArgumentException("Missing data values");
            else if (colnames.Count == 0 && datavalues.Count > 0)
                throw new ArgumentException("Missing column names");
            // add it so we can use a loop
            if (datavalues.Count == 0)
                datavalues.Add(new List<string>());

            foreach (var q in datavalues)
            {
                var context = StoryType.BaseType.GetGenericArguments()[0].GetConstructor(System.Type.EmptyTypes).Invoke(null);

                yield return new TestResult(ReplaceStringsWithOtherStrings(ScenarioType.Name, colnames, q)
                        , tr => tr.Children.All(itr => itr.Status == true)
                        , () => (StoryType.GetFields(defaultflags)
                                .Where(mi => mi.FieldType.Name == "Given")
                                .Select(o => RunDelegate(o, context, colnames, q)))
                                .Union(ScenarioType.GetFields(defaultflags)
                                    .Where(m2 => m2.FieldType.Name == "Given")
                                    .Select(o => RunDelegate(o, context, colnames, q)))
                                .Union(ScenarioType.GetFields(defaultflags)
                                    .Where(m2 => m2.FieldType.Name == "When")
                                    .Select(o => RunDelegate(o, context, colnames, q)))
                                .Union(ScenarioType.GetFields(defaultflags)
                                    .Where(m2 => m2.FieldType.Name == "Then")
                                    .Select(o => RunDelegate(o, context, colnames, q)))
                        );
            }

            // should never happen
        }

        public ITestResult RunDelegate(FieldInfo fi, object Context, List<string> colnames, List<string> replacements)
        {
            var q = ReplaceStringsWithOtherStrings(fi.Name, colnames, replacements);

            return new TestResult(fi.FieldType.Name + " " + q
                , o => { 
                    var result = (bool?)CallContextDSL(q, Context,fi.FieldType.Name);
                    return fi.FieldType.Name == "Then" ? result : true;
                }
                , () => new ITestResult[] { }
                );
        }

        public static object CallContextDSL(string name, object context,string phasename)
        {
            object returnresult = null;
            foreach (var mi in context.GetType().GetMethods(defaultflags))
                foreach (var attr in mi.GetCustomAttributes(typeof(DSLAttribute), true))
                {
                    var regex = ((DSLAttribute)attr).MyRegEx ?? mi.Name;
                    var match = Regex.Match(name, "^" + regex + "$");
                    if (!match.Success)
                        continue;
                    var pars = new List<object>();
                    foreach (var pi in mi.GetParameters())
                    {
                        var strval = match.Groups[pi.Name].Value;
                        object result = null;
                        if (strval == match.Value)
                            result = Convert.ChangeType(strval, pi.ParameterType);
                        else
                            try
                            {
                                result = CallContextDSL(strval, context, "ghkazbnkazbkeaz");
                            } catch(Exception)
                            {
                                if (pi.ParameterType.IsEnum)
                                    result = Enum.Parse(pi.ParameterType,strval,true);
                                else
                                    result = Convert.ChangeType(strval, pi.ParameterType);
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
                            context.Set(phasename + "Exception",exc);
                            return returnresult;
                        }
                        catch (Exception)
                        {

                        }
                        throw;
                    }
                    return returnresult;
                }
            throw new NotImplementedException(string.Format("DSL<{0}> : {1}", context.GetType(), name));
        }

        public static string ReplaceStringsWithOtherStrings(string src, List<string> stringstoreplace, List<string> replacements)
        {
            for (var i = 0; i < stringstoreplace.Count; i++)
            {
                src = src.Replace(stringstoreplace[i], replacements[i]);
            }
            return src;

        }

    }
}
