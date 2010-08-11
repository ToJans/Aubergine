using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aubergine.Interfaces
{
    public interface ISpecElement
    {
        string Description { get; set; }
        bool? Status { get; set; }
        IEnumerable<ISpecElement> Children { get;set;} 
        string StatusInfo { get; set; }
        ElementType Type{get;}
        ISpecElement Parent { get; set; }
        ISpecElement Clone();
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
        Data,
        Context
    }


}
