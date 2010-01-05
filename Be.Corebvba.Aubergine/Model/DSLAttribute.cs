using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Corebvba.Aubergine.Model
{
    [AttributeUsage(AttributeTargets.Method,AllowMultiple=true)]
    public class DSLAttribute : System.Attribute
    {
        public string MyRegEx { get; set; }
        public string TableParameterName { get; set; }
        public DSLAttribute()
        { }
        public DSLAttribute(string regex)
        {
            MyRegEx = regex;
        }
        public DSLAttribute(string regex, string tableParameterName) : this(regex)
        {
            TableParameterName = tableParameterName;
        }
    }

    public class GivenAttribute : DSLAttribute { }
    public class WhenAttribute : DSLAttribute { }
    public class ThenAttribute : DSLAttribute { }
}
