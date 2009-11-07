using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Corebvba.Aubergine
{
    public static class Extensions
    {
        public static void ShouldEqual(this object o1, object o2)
        {
            bool isequal = false;
            if (o1 == null)
            {
                isequal = (o2 == null);
            }
            else
            {
                isequal = o1.Equals(o2);
            }
            if (!isequal)
                throw new Exception("Values are not equal");
        }

        public static T As<T>(this string o1)
        {
            return (T)Convert.ChangeType(o1, typeof(T));
        }

        public static T Get<T>(this object o, string name)
        {
            var t = o.GetType();
            var x = t.GetProperty(name);
            if (x != null)
                return (T)x.GetValue(o, null);
            var y = t.GetField(name);
            if (y != null)
                return (T)y.GetValue(o);
            throw new ArgumentOutOfRangeException("Unknown field/property : " + name);
        }

        public static void Set(this object o, string name, object value)
        {
            var t = o.GetType();
            var x = t.GetProperty(name);
            if (x != null)
            {
                x.SetValue(o, value, null);
                return;
            }
            var y = t.GetField(name);
            if (y != null)
            {
                y.SetValue(o, value);
                return;
            }
            throw new ArgumentOutOfRangeException("Unknown field/property : " + name);
        }
    }
}
