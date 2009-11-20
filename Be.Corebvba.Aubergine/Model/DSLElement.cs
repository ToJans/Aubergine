using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Be.Corebvba.Aubergine.Interfaces;
using System.Reflection;

namespace Be.Corebvba.Aubergine.Model
{
    public class DSLParameter : IDSLParameter
    {

        public string Name {get;set;}
        public Type CLRType { get; set; }

    }

    public class DSLElement : IDSLElement
    {
        private MethodInfo _mi;

        public DSLElement(string regex,MethodInfo mi)
        {
            _mi = mi;
            RegEx = new Regex(regex, RegexOptions.IgnoreCase);
            ReturnType = mi.ReturnType;
            Parameters = mi.GetParameters().Select(p => (IDSLParameter)new DSLParameter()
            {
                CLRType = p.ParameterType,
                Name = p.Name.ToLower()
            });
            Description = ReturnType.Name + " " 
                + regex.Substring(1).Substring(0,regex.Length -2) + "(" 
                + string.Join(",", Parameters.Select(o => o.CLRType.Name + " " + o.Name).ToArray()) + ")";
        }

        public Regex RegEx { get; set; }
        public IEnumerable<IDSLParameter> Parameters { get; set; }
        public Type ReturnType {get;set;}
        public object Invoke(object context,object[] pars)
        {
            return _mi.Invoke(context, pars);
        }

        public string Description { get; private set; }
    }
}
