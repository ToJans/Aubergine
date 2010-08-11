using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aubergine.Tests.Accounts.SoftwareToTest.Controllers;

namespace Aubergine.Tests.Accounts.SoftwareToTest.ViewModels
{
    public class ViewModel
    {
        public string Message { get; set; }
        public ReturnResult Result {get;set;}
    }

    public class LoginViewModel:ViewModel
    {
        public Action LoginButtonClicked;
        public string Username {get;set;}
        public string Password {get;set;}
    }
}
