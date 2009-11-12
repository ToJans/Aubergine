using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Be.Corebvba.Aubergine.Interfaces;
using Be.Corebvba.Aubergine.Model;
using Be.Corebvba.Aubergine.ClassStory;

namespace Be.Corebvba.Aubergine.Services.Extractors
{
    public class TextExtractor : IStoryExtractor
    {
        #region IStoryExtractor Members

        public IEnumerable<IStoryContainer> ExtractStories(string files)
        {
            IElement LastElement = null;
            IElement LastElementContainer = null;
            var StoryContainer = new List<IStoryContainer>();
            var StoryChildren= new List<IElement>();
            var ScenarioChildren = new List<IElement>();
            var DataChildren = new List<IElement>();
            string context = null;
            string contextdll = null;
            string columntoken = "|";

            foreach (var vv in File.ReadAllLines(files))
            {
                if (vv == null) continue;
                var v= vv.Trim();
                if (v == "") continue;
                if (v.ToLower().StartsWith("context "))
                {
                    context = v.Substring("context ".Length).Trim();
                    continue;
                }
                if (v.ToLower().StartsWith("contextdll "))
                {
                    contextdll = v.Substring("contextdll ".Length).Trim();
                    continue;
                }

                if (v.ToLower().StartsWith("columntoken "))
                {
                    columntoken = v.Substring("columntoken ".Length).Trim();
                    continue;
                }


                var q =
                    TransformText(v, "story ", ElementType.Story) ??
                    TransformText(v, "scenario ", ElementType.Scenario) ??
                    TransformText(v, "given ", ElementType.Given) ??
                    TransformText(v, "when ", ElementType.When) ??
                    TransformText(v, "then ", ElementType.Then) ??
                    TransformText(v, "example", ElementType.Example) ??
                    TransformText(v, columntoken, ElementType.Data);
                
                if (q == null && LastElement == null) 
                    continue;

                q = q??TransformText(v, "and ", LastElement.Type);

                if (q == null) continue;


                switch (q.Type)
                {
                    case ElementType.Story:
                        StoryContainer.Add(new StoryContainer() { Story = q,ContextType = GetTypeFromAssembly(context,contextdll),ColumnToken = columntoken });
                        StoryChildren = new List<IElement>();
                        ScenarioChildren = new List<IElement>();
                        DataChildren = new List<IElement>();
                        q.Children = StoryChildren;
                        LastElementContainer = q;
                        break;
                    case ElementType.Scenario:
                        StoryChildren.Add(q);
                        ScenarioChildren = new List<IElement>();
                        DataChildren = new List<IElement>();
                        q.Children = ScenarioChildren;
                        LastElementContainer = q;
                        break;
                    case ElementType.Given:
                        if (LastElementContainer.Type == ElementType.Story)
                            StoryChildren.Add(q);
                        else
                            ScenarioChildren.Add(q);
                        DataChildren = new List<IElement>();
                        q.Children = DataChildren;
                        break;
                    case ElementType.Then:
                        ScenarioChildren.Add(q);
                        DataChildren = new List<IElement>();
                        q.Children = DataChildren;
                        break;
                    case ElementType.When:
                        ScenarioChildren.Add(q);
                        DataChildren = new List<IElement>();
                        q.Children = DataChildren;
                        break;
                    case ElementType.Example:
                        ScenarioChildren.Add(q);
                        DataChildren = new List<IElement>();
                        q.Children = DataChildren;
                        break;
                    case ElementType.Data:
                        q.Description = columntoken + q.Description;
                        DataChildren.Add(q);
                        break;
                }
                if (q.Type!= ElementType.Data)
                    LastElement = q;
            }
            return StoryContainer;
        }


        static IElement TransformText(string input, string match,ElementType ret)
        {
            if (input.ToLower().StartsWith(match))
            {
                return new Element(ret,input.Substring(match.Length));
            }
            return null;
        }

        static Type GetTypeFromAssembly(string context, string contextdll)
        {
            if (string.IsNullOrEmpty(context))
                throw new ArgumentNullException("Context is not defined");
            if (string.IsNullOrEmpty(contextdll))
                throw new ArgumentNullException("ContextDLL is not defined");
            var fi = new FileInfo(contextdll);
            var asm = System.Reflection.Assembly.LoadFile(fi.FullName);
            return asm.GetType(context);
        }

        #endregion
    }
}
