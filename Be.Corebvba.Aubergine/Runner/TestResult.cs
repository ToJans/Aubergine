using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Corebvba.Aubergine.Runner
{
    class TestResult : ITestResult
    {

        public string Description { get; private set; }
        private Func<ITestResult, bool> StatusFunc;
        private Func<IEnumerable<ITestResult>> childfunc;
        private Func<object> internalcontextprovider;
        private IEnumerable<ITestResult> internalchildren;
        private object context;

        private bool? internalstatus;

        public bool Status 
        {
            get {
                    if (internalstatus == null)
                    {
                        try
                        {
                            internalstatus = StatusFunc(this);
                            foreach (var x in Children)
                            {
                                internalstatus &= x.Status;
                            }
                        }
                        catch (Exception ex)
                        {
                            internalstatus = false;
                            ExtraStatusInfo = "";
                            while (ex.InnerException != null)
                                ex = ex.InnerException;
                            ExtraStatusInfo = ex.Message;
                        }
                    }
                    return internalstatus ?? false;
                }
        }


        public TestResult(string description, Func<ITestResult,bool> runner, Func<IEnumerable<ITestResult>> children)
        {
            Description = description;
            StatusFunc = runner;
            childfunc = children;
            //internalcontextprovider = contextprovider;
        }

        #region IRunResults Members

        public IEnumerable<ITestResult> Children
        {
            get {
                if (internalchildren == null)
                {
                    internalchildren = childfunc();
                }
                return internalchildren;
            }
        }

        #endregion

        #region ITestResult Members


        public string ExtraStatusInfo { get; set; }

        #endregion
    }
}
