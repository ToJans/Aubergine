using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Corebvba.Aubergine.Interfaces
{
    public interface IDSLRunner
    {
        object GetNewContext();
        object CallContextDSL(string name, object context, string phasename, Func<Dictionary<string, List<string>>> GetTable);
    }
}
