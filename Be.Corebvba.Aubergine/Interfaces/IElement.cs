using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Corebvba.Aubergine.Interfaces
{
    public interface IElement
    {
        string Description { get; set; }
        bool? Status { get; set; }
        IEnumerable<IElement> Children { get;set;} 
        string StatusInfo { get; set; }
        ElementType Type{get;}
        IElement Parent { get; set; }
        IElement Clone();
        string StatusText { get; }
    }

    public enum ElementType
    {
        Story,
        Scenario,
        GivenIdid,
        Given,
        When,
        Then,
        Example,
        Data
    }


}
