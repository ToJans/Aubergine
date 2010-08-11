using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aubergine.Tests.Accounts.SoftwareToTest.Model;
using Aubergine.Tests.Accounts.SoftwareToTest.Services;
using Aubergine.Tests.Accounts.SoftwareToTest.ViewModels;

namespace Aubergine.Tests.Accounts.SoftwareToTest.Controllers
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
