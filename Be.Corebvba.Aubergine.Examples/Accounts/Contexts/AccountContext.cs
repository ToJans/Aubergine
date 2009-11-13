using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Be.Corebvba.Aubergine.Model;
using Be.Corebvba.Aubergine.Extensions;
using Be.Corebvba.Aubergine.Examples.Accounts.SoftwareToTest.Model;
using Be.Corebvba.Aubergine.Examples.Accounts.SoftwareToTest.Services;

namespace Be.Corebvba.Aubergine.Examples.Accounts.Contexts
{
    internal class AccountContext
    {
        public User currentUser = new User();
        public Account AccountA = new Account();
        public Account AccountB = new Account();
        public AccountService AccountService = new AccountService();
        public ProcessStatus LastStatus;

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

        [DSL(@"I request authentication for (?<user>.+)")]
        void authenticate_for_account_x(User user)
        {
            LastStatus = AccountService.AuthenticateUser(user);
        }

        [DSL(@"I request authentication for (?<account>.+) with (?<user>.+)")]
        void authenticate_for_account_x(User user, Account account)
        {
            LastStatus =  AccountService.AuthenticateUserForAccount(account,user);
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

        [DSL("(?<name>Account[AB])")]
        Account getaccountAB(string name)
        {
            return this.Get<Account>(name);
        }

        [DSL(@"(?<amount>\d+)(?<ismillion>m?)")]
        decimal getmillion(decimal amount,string  ismillon)
        {
            return amount*(ismillon=="m"?1m:1);
        }

        [DSL]
        User the_current_user()
        {
            return currentUser;
        }
    }
}
