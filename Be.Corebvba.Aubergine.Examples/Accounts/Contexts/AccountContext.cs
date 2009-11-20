using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Be.Corebvba.Aubergine.Model;
using Be.Corebvba.Aubergine.Extensions;
using Be.Corebvba.Aubergine.Examples.Accounts.SoftwareToTest.Model;
using Be.Corebvba.Aubergine.Examples.Accounts.SoftwareToTest.Services;
using Be.Corebvba.Aubergine.Examples.Accounts.SoftwareToTest.Controllers;
using Be.Corebvba.Aubergine.Examples.Accounts.SoftwareToTest.ViewModels;

namespace Be.Corebvba.Aubergine.Examples.Accounts.Contexts
{
    internal class AccountContext
    {
        public GenericRepository<User> rUser;
        public GenericRepository<Account> rAccount;
        public AccountService AccountService;
        public ProcessStatus LastStatus;


        public ControllerBuilder cb;
        public BaseController LastController;

        public AccountContext()
        {
            rUser = new GenericRepository<User>((u,n)=>u.Name ==n);
            rAccount = new GenericRepository<Account>((a,n)=>a.Number==n);
            AccountService = new AccountService(rUser, rAccount);
            cb = new ControllerBuilder(AccountService);
        }

        [DSL(@"the (?<member>.+) of (?<instance>.+) is (?<value>.+)")]
        void assignfield(object instance,string member,object value)
        {
            instance.Set(member, value);
        }

        [DSL(@"the (?<member>.+) of (?<instance>.+) should be (?<value>.*)")]
        bool shouldbefield(object instance, string member, object value)
        {
            var obj = instance.Get<object>(member);
            return Convert.ChangeType(value, obj.GetType()).Equals(obj);
        }

        [DSL]
        void the_following_users(string[] username, string[] password)
        {
            for (var i = 0; i < username.Length; i++)
                rUser.Add(new User() { Name = username[i], Password = password[i] });
        }

        [DSL]
        void the_following_accounts(string[] number, decimal[] balance,User[] owner)
        {
            for (var i = 0; i < number.Length; i++)
                rAccount.Add(new Account() { Number = number[i], Balance = balance[i],Owner = owner[i] });
        }

        [DSL(@"the user (?<username>.*)")]
        User GetUser(string username)
        {
            return rUser.Find(username);
        }

        [DSL(@"I transfer (?<amount>.+) from (?<from>.+) to (?<to>.+) with (?<user>.+)")]
        void transfering_xm_from_a_to_b(decimal amount, Account from, Account to,User user)
        {
            LastStatus = AccountService.Transfer(user,amount, from,to);
        }

        [DSL]
        bool The_process_should_fail()
        {
            return LastStatus.Success == false;
        }

        [DSL]
        bool The_process_should_succeed()
        {
            return LastStatus.Success == true;
        }

        [DSL(@"(?<amount>\d+)(?<ismillion>m?)")]
        decimal getmillion(decimal amount,string  ismillon)
        {
            return amount*(ismillon=="m"?1m:1);
        }

        [DSL(@"I go to the (?<screenname>.+) screen")]
        void Navigate(string screenname)
        {
            if (screenname.ToLower().Trim() == "login")
            {
                LastController  = cb.GetController<LoginController>(x=>x.Login());
            }
            else
                throw new ArgumentException("Unknown screen : " + screenname);
        }

        [DSL(@"I enter (?<value>.+) as the (?<member>.+)")]
        void assignscreenfield(string member, object value)
        {
             LastController.Result.ViewModel.Set(member, value);
        }

        [DSL(@"I click the (?<element>.*)")]
        void click(string element)
        {
            var x = LastController as LoginController;
            if (x!=null)
            {
                if (element.ToLower().Trim() == "login button")
                {
                    x.ValidateLogin();
                    return;
                }
            }
            throw new ArgumentException("Unknown element in view : " + element);
        }

        [DSL]
        ViewModel the_result()
        {
            return LastController.Result.ViewModel;
        }

        [DSL(@"account (?<account>.*)")]
        Account GetAccount(string account)
        {
            return rAccount.Find(account.Trim());
        }

    }
}
