using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Be.Corebvba.Aubergine.Interfaces;

namespace Be.Corebvba.Aubergine.Interfaces
{
    public interface IStoryExtractor
    {
        IEnumerable<IStoryContainer> ExtractStories(string files);
    }
}
