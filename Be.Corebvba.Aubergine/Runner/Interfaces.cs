using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Corebvba.Aubergine.Runner
{
    public interface ITestResult
    {
        string Description {get;}
        bool Status {get;}
        IEnumerable<ITestResult> Children {get;}
        string ExtraStatusInfo { get; set; }
    }
}
