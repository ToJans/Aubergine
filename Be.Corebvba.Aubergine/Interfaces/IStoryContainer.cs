using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Corebvba.Aubergine.Interfaces
{
    public interface IStoryContainer
    {
        Type ContextType { get; set; }
        IElement Story { get; set; }
        string ColumnToken { get; set; }
    }
}
