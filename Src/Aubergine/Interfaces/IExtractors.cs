using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aubergine.Interfaces;

namespace Aubergine.Interfaces
{
    public interface IExtractor
    {
        bool ParseLine(string line);
    }

    public interface IDSLExtractor : IExtractor
    {
        Dictionary<string, IDSLDefinition> DSLs { get; }
    }

    public interface IStoryExtractor : IExtractor 
    {
        IEnumerable<IStoryContainer> Stories { get; }
    }
}
