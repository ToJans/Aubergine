using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Aubergine.Model;
using Aubergine.Extensions;
using Aubergine.Interfaces;
using System.Text.RegularExpressions;

namespace Aubergine.Services
{
    public class DSLRunner : IDSLRunner
    {

        IDSLDefinition DslDef;

        public DSLRunner(IDSLDefinition dsldef)
        {
            DslDef = dsldef;
        }


        public object CallContextDSL(string name, object context, string phasename, Func<Dictionary<string, List<string>>> GetTable)
        {
            object returnresult = null;
            var tbl = GetTable();
            name = name.Trim();
            foreach(var dsl in DslDef.Elements)
            {
                var match = dsl.RegEx.Match(name);
                if (!match.Success) continue;
                var pars = new List<object>();
                try
                {
                    foreach (var pi in dsl.Parameters)
                    {
                        var strval = match.Groups[pi.Name].Value;
                        object result = null;
                        var lname = pi.Name.ToLower();
                        if (tbl != null && tbl.Keys.Count > 0 && tbl.ContainsKey(lname) && pi.CLRType.IsArray)
                        {
                            var eltype = pi.CLRType.GetElementType();
                             var arr = Array.CreateInstance(eltype, tbl[lname].Count);
                             for (int i = 0; i < arr.Length; i++)
                             {
                                 arr.SetValue(ConvertTypeForDSL(tbl[lname][i], context, eltype), i);
                             }
                             result = arr;
                        }
                        else
                        {
                            if (strval == match.Value)
                                result = Convert.ChangeType(strval, pi.CLRType);
                            else
                            {
                                result = ConvertTypeForDSL(strval, context, pi.CLRType);
                            }
                        }
                        pars.Add(result);
                    }
                }
                catch (Exception)
                {
                    // if the parameter conversion fails try to execute the next DSL match
                    continue;
                }

                // call the DSL function
                try
                {
                    returnresult = dsl.Invoke(context, pars.ToArray());
                }
                catch (Exception exc)
                {
                    try
                    {
                        // if exception field is provided, catch it in this field, else rethrow
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
            throw new NotImplementedException(string.Format("Unknown / missing DSL:\n{1}", context.GetType(), name));
        }
    
        private object ConvertTypeForDSL(string strval, object context, Type desttype)
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


        public object GetNewContext()
        {
            return Activator.CreateInstance(DslDef.ContextType);
        }

    }
}
