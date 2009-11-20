using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Be.Corebvba.Aubergine.Interfaces;
using Be.Corebvba.Aubergine.Model;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Be.Corebvba.Aubergine.Services.Extractors
{
    public class TextStoryExtractor : IStoryExtractor
    {
        ISpecElement LastElement = null;
        ISpecElement LastElementContainer = null;
        List<IStoryContainer> StoryContainers = new List<IStoryContainer>();
        List<ISpecElement> StoryChildren = new List<ISpecElement>();
        List<ISpecElement> ScenarioChildren = new List<ISpecElement>();
        List<ISpecElement> DataChildren = new List<ISpecElement>();
        string columntoken = "|";

        public bool ParseLine(string vv)
        {
            if (vv == null) return true;
            var v = vv.Trim();
            if (v == "") return true;

            if (v.ToLower().StartsWith("columntoken "))
            {
                columntoken = v.Substring("columntoken ".Length).Trim();
                return true;
            }

            var q =
                TransformText(v, "story ", ElementType.Story) ??
                TransformText(v, "scenario ", ElementType.Scenario) ??
                TransformText(v, "given i did ", ElementType.GivenIdid) ??
                TransformText(v, "given ", ElementType.Given) ??
                TransformText(v, "when ", ElementType.When) ??
                TransformText(v, "then ", ElementType.Then) ??
                TransformText(v, "example", ElementType.Example) ??
                TransformText(v, "is about ",ElementType.Context) ??
                TransformText(v, columntoken, ElementType.Data);

            if (q == null && LastElement == null)
                return false;

            q = q ?? TransformText(v, "and ", LastElement.Type);

            if (q == null) return false;


            switch (q.Type)
            {
                case ElementType.Story:
                    StoryContainers.Add(new StoryContainer()
                    {
                        Story = q,
                        ColumnToken = columntoken
                    });

                    StoryChildren = new List<ISpecElement>();
                    ScenarioChildren = new List<ISpecElement>();
                    DataChildren = new List<ISpecElement>();
                    q.Children = StoryChildren;
                    LastElementContainer = q;
                    break;
                case ElementType.Scenario:
                    StoryChildren.Add(q);
                    ScenarioChildren = new List<ISpecElement>();
                    DataChildren = new List<ISpecElement>();
                    q.Children = ScenarioChildren;
                    LastElementContainer = q;
                    break;
                case ElementType.Given:
                    if (LastElementContainer.Type == ElementType.Story)
                        StoryChildren.Add(q);
                    else
                        ScenarioChildren.Add(q);
                    DataChildren = new List<ISpecElement>();
                    q.Children = DataChildren;
                    break;
                case ElementType.GivenIdid:
                    ScenarioChildren.Add(q);
                    DataChildren = new List<ISpecElement>();
                    q.Children = DataChildren;
                    break;
                case ElementType.Then:
                    ScenarioChildren.Add(q);
                    DataChildren = new List<ISpecElement>();
                    q.Children = DataChildren;
                    break;
                case ElementType.When:
                    ScenarioChildren.Add(q);
                    DataChildren = new List<ISpecElement>();
                    q.Children = DataChildren;
                    break;
                case ElementType.Example:
                    ScenarioChildren.Add(q);
                    DataChildren = new List<ISpecElement>();
                    q.Children = DataChildren;
                    break;
                case ElementType.Data:
                    q.Description = columntoken + q.Description;
                    DataChildren.Add(q);
                    break;
                case ElementType.Context:
                    StoryChildren.Add(q);
                    break;
 
            }
            if (q.Type != ElementType.Data)
                LastElement = q;
            return true;
        }

        #region IStoryExtractor Members

        public IEnumerable<IStoryContainer> Stories
        {
            get
            {
                return StoryContainers;
            }
        }


        static ISpecElement TransformText(string input, string match, ElementType ret)
        {
            if (input.ToLower().StartsWith(match))
            {
                return new SpecElement(ret, input.Substring(match.Length));
            }
            return null;
        }

        #endregion

        #region IExtractor Members

        #endregion
    }
}
