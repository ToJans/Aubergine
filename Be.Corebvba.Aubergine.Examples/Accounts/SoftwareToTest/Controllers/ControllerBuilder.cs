using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Be.Corebvba.Aubergine.Examples.Accounts.SoftwareToTest.Services;

namespace Be.Corebvba.Aubergine.Examples.Accounts.SoftwareToTest.Controllers
{
    public class ControllerBuilder
    {
        private AccountService _sAccount;
        public ControllerBuilder(AccountService sAccount)
        {
            _sAccount = sAccount;
        }

        public T GetController<T>() where T:BaseController
        {
            if (typeof(T) == typeof(LoginController))
                return new LoginController(this,_sAccount) as T;
            if (typeof(T) == typeof(AccountController))
                return new AccountController(this) as T;
            return null;
        }

        public T GetController<T>(Action<T> act) where T : BaseController
        {
            var q = GetController<T>();
            if (q != null)
                act(q);
            return q;
        }
    }
}
