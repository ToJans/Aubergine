using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Corebvba.Aubergine
{
    public class DSLAttribute : System.Attribute
    {
        public string MyRegEx { get; set; }
        public DSLAttribute()
        { }
        public DSLAttribute(string regex)
        {
            MyRegEx = regex;
        }
    }
}
