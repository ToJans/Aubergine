using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Corebvba.Aubergine.Model
{
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
}
