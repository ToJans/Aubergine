using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Corebvba.Aubergine
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ColsAttribute : System.Attribute
    {
        public string[] Names {get;private set;}

        public ColsAttribute(params string[] names)
        {
            Names = names;
        }
    }

}
