using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Corebvba.Aubergine
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DataAttribute : System.Attribute
    {
        public Object[] Vals { get; private set; }
        public DataAttribute(params object[] objs)
        {
            Vals = objs;
        }
    }
}
