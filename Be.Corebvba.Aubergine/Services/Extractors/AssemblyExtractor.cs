using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using Be.Corebvba.Aubergine.Interfaces;
using Be.Corebvba.Aubergine.Model;
using Be.Corebvba.Aubergine.ClassStory;

namespace Be.Corebvba.Aubergine.Services.Extractors
{
    public class AssemblyExtractor : IStoryExtractor 
    {
        const BindingFlags defaultflags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        string ColumnToken = Guid.NewGuid().ToString();

        public IEnumerable<IStoryContainer> ExtractStories(string file)
        {
                var asm = Assembly.LoadFile(new System.IO.FileInfo( file).FullName);
                foreach (var tp in asm.GetTypes().Where(t=>t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == typeof(Story<>)))
                {
                    var sc = new StoryContainer();
                    sc.ColumnToken = ColumnToken;
                    sc.ContextType = tp.BaseType.GetGenericArguments()[0];
                    sc.Story = new Element(ElementType.Story, tp.Name);
                    sc.Story.Children = GetChildren(tp, sc.Story);
                    yield return sc;
                }
        }


        IEnumerable<IElement> GetChildren(Type t, IElement Parent)
        {
            var l = new List<IElement>();
            var newtypestoget = new List<ElementType>();
            switch (Parent.Type)
            {
                case ElementType.Story:
                    newtypestoget.Add(ElementType.Given);
                    foreach (var sc in t.GetNestedTypes(defaultflags).Where(it=>it.BaseType.Name == ElementType.Scenario.ToString()))
                    {
                        var el = new Element(ElementType.Scenario,sc.Name);
                        el.Children = GetChildren(sc,el);
                        l.Add(el);
                    }
                    break;
                case ElementType.Scenario:
                    foreach (var example in GetExamples(t))
                        l.Add(example);
                    newtypestoget.Add(ElementType.Given);
                    newtypestoget.Add(ElementType.When);
                    newtypestoget.Add(ElementType.Then);
                    break;
            }
            foreach (var nt in newtypestoget)
            {
                foreach (var ct in t.GetFields(defaultflags).Where(it => it.FieldType.Name == nt.ToString()))
                    l.Add(new Element(nt, ct.Name));
            }
            return l;
        }

        IEnumerable<IElement> GetExamples(Type t)
        {
            var l = new List<IElement>();
            var cols = t.GetCustomAttributes(typeof(ColsAttribute),true).Cast<ColsAttribute>().FirstOrDefault();
            if (cols == null) 
                yield break;
            yield return new Element(ElementType.Example, ColumnToken + string.Join(ColumnToken, cols.Names)+ColumnToken);
            foreach (var dat in t.GetCustomAttributes(typeof(DataAttribute), true).Cast<DataAttribute>())
            {
                yield return new Element(ElementType.Example, ColumnToken+string.Join(ColumnToken, dat.Vals.Select(v=>v.ToString()).ToArray())+ColumnToken);
            }
        }


    }
}
