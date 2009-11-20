using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Be.Corebvba.Aubergine.Interfaces
{

    public interface IDSLParameter
    {
        string Name { get; }
        Type CLRType { get; }
    }

    public interface IDSLElement
    {
        Regex RegEx { get; }
        IEnumerable<IDSLParameter> Parameters { get; }
        Type ReturnType { get; set; }
        object Invoke(object context, object[] pars);
        string Description { get; }
    }

    public interface IDSLDefinition
    {
        string Name { get; }
        Type ContextType { get; }
        IEnumerable<IDSLElement> Elements { get; }
    }
}
