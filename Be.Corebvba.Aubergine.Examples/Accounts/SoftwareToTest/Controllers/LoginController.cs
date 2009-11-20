using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Be.Corebvba.Aubergine.Examples.Accounts.SoftwareToTest.Model;
using Be.Corebvba.Aubergine.Examples.Accounts.SoftwareToTest.Services;
using Be.Corebvba.Aubergine.Examples.Accounts.SoftwareToTest.ViewModels;

namespace Be.Corebvba.Aubergine.Examples.Accounts.SoftwareToTest.Controllers
{
    public class LoginController : BaseController
    {
        AccountService _sAccount;

        public LoginController(ControllerBuilder cb, AccountService sAccount)
            : base(cb)
        {
            _sAccount = sAccount;
        }

        public void Login()
        {
            var vm = (this.Result.ViewModel as LoginViewModel) ??new LoginViewModel();
            this.Result.ViewModel = vm;
        }

        public void ValidateLogin()
        {
            var vm = (this.Result.ViewModel as LoginViewModel) ?? new LoginViewModel();
            var u = new User();
            var msg = _sAccount.ValidateUser(vm.Username, vm.Password, ref u);
            Result.ViewModel.Message = msg.Message;
            if (!msg.Success)
                Response<AccountController>(f => f.Index(u));
            else
                Response<LoginController>(f => f.Login());
        }
    }
}
