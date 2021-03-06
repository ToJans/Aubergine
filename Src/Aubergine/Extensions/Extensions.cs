﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Reflection;

namespace Aubergine.Extensions
{
    public static class ExtensionsImpl
    {
        const BindingFlags defaultflags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public;

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
            var x = t.GetProperty(name,defaultflags);
            if (x != null)
                return (T)x.GetValue(o, null);
            var y = t.GetField(name, defaultflags);
            if (y != null)
                return (T)y.GetValue(o);
            throw new ArgumentOutOfRangeException("Unknown field/property : " + name);
        }

        public static void Set(this object o, string name, object value)
        {
            var t = o.GetType();
            var x = t.GetProperty(name, defaultflags);
            if (x != null)
            {
                var q = value;
                if (q.GetType() != x.PropertyType) q = Convert.ChangeType(value, x.PropertyType);
                x.SetValue(o, q, null);
                return;
            }
            var y = t.GetField(name, defaultflags);
            if (y != null)
            {
                var q = value;
                if (q.GetType() != y.FieldType) q = Convert.ChangeType(value, y.FieldType);
                y.SetValue(o, q);
                return;
            }
            throw new ArgumentOutOfRangeException("Unknown field/property : " + name);
        }

        public static string FormatWith(this string format, object source)
        {

            return FormatWith(format, null, source);

        }



        public static string FormatWith(this string format, IFormatProvider provider, object source)
        {

            if (format == null)

                throw new ArgumentNullException("format");



            Regex r = new Regex(@"(?<start>\{)+(?<property>[\w\.\[\]]+)(?<format>:[^}]+)?(?<end>\})+",

              RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);



            List<object> values = new List<object>();

            string rewrittenFormat = r.Replace(format, delegate(Match m)
            {

                Group startGroup = m.Groups["start"];

                Group propertyGroup = m.Groups["property"];

                Group formatGroup = m.Groups["format"];

                Group endGroup = m.Groups["end"];



                values.Add((propertyGroup.Value == "0")

                  ? source

                  : DataBinder.Eval(source, propertyGroup.Value));



                return new string('{', startGroup.Captures.Count) + (values.Count - 1) + formatGroup.Value

                  + new string('}', endGroup.Captures.Count);

            });



            return string.Format(provider, rewrittenFormat, values.ToArray());

        }
    }
}
