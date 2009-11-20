using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Be.Corebvba.Aubergine.Interfaces;
using Be.Corebvba.Aubergine.Model;
using System.IO;
using System.Reflection;

namespace Be.Corebvba.Aubergine.Services.Extractors
{
    public class TextDSLExtractor : IDSLExtractor 
    {
        const BindingFlags defaultflags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static |
BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.GetField | BindingFlags.SetField;



        private Dictionary<string,string> AsmNames = new Dictionary<string,string>();
        private Dictionary<string, string> ClsNames = new Dictionary<string,string>();
        private string ctxname;

        public Dictionary<string,IDSLDefinition> DSLs 
        { 
            get
            {
                var d = new Dictionary<string, IDSLDefinition>();
                foreach (var k in ClsNames.Keys)
                {
                    d.Add(k,ExtractDSL(k,GetTypeFromAssembly(ClsNames[k],AsmNames[k])));
                }
                return d;
            }
        }

        public TextDSLExtractor()
        {
        }

        public void AddDSL(string name, string clsname, string asmname)
        {
            AsmNames.Add(name, asmname);
            ClsNames.Add(name, clsname);
        }

        public bool ParseLine(string line)
        {
            line = parsepart(line, "define ", s => { ctxname = s.Trim(); AsmNames.Add(ctxname, "Unknown assembly"); ClsNames.Add(ctxname, "Unknown class"); });
            line = parsepart(line, "from ", s => AsmNames[ctxname] = s.Trim());
            line = parsepart(line, "using ", s => ClsNames[ctxname] = s.Trim());
            return line == "";
        }

        private static string parsepart(string input, string part, Action<string> DoIt)
        {
            var i = input.Trim();
            if (i.ToLower().StartsWith(part))
            {
                DoIt(i.Substring(part.Length));
                return "";
            }
            else
            {
                return input;
            };
        }

        static Type GetTypeFromAssembly(string context, string contextdll)
        {
            if (string.IsNullOrEmpty(context))
                throw new ArgumentNullException("Context is not defined");
            if (string.IsNullOrEmpty(contextdll))
                throw new ArgumentNullException("ContextAssembly is not defined");
            var asm = System.Reflection.Assembly.LoadFrom(contextdll);
            return asm.GetType(context);
        }

        private static Dictionary<string, IDSLDefinition> dsls = new Dictionary<string, IDSLDefinition>();

        private IDSLDefinition ExtractDSL(string dslname, Type t)
        {
            var fullname = t.FullName+dslname;
            if (!dsls.ContainsKey(fullname))
            {
                var l = new List<IDSLElement>();
                foreach (var mi in t.GetMethods(defaultflags))
                    foreach (var attr in mi.GetCustomAttributes(typeof(DSLAttribute), true))
                    {
                        var r = ((DSLAttribute)attr).MyRegEx;
                        if (string.IsNullOrEmpty(r))
                        {
                            r = mi.Name.Replace("_", "[ _]");
                        }
                        l.Add(new DSLElement("^" + r + "$", mi));
                    }
                dsls.Add(fullname, new DSLDefinition() { ContextType = t, Elements = l,Name = dslname });
            }
            return dsls[fullname];
        }




    }
}
