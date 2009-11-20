using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Corebvba.Aubergine.Examples.Accounts.SoftwareToTest.Controllers
{
    public class BaseController
    {
        private ControllerBuilder _cb;

        public BaseController(ControllerBuilder cb)
        {
            _cb = cb;
            Result = new ReturnResult();
        }

        public void Response<T>(Action<T> redir) where T:BaseController 
        {
            var ctl = _cb.GetController<T>(q =>  q.Result = this.Result );
            redir(ctl);
            this.Result = ctl.Result??this.Result;
        }

        public ReturnResult Result { get; set; }
    }
}
