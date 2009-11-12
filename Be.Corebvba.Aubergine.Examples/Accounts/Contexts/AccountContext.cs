using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Be.Corebvba.Aubergine.Model;
using Be.Corebvba.Aubergine.Extensions;
using Be.Corebvba.Aubergine.Examples.Accounts.SoftwareToTest;

namespace Be.Corebvba.Aubergine.Examples.Accounts.Contexts
{
    internal class AccountContext
    {
        public Account AccountA = new Account();
        public Account AccountB = new Account();
        public Exception WhenException;

        #region Given

        [DSL(@"(?<account>Account[AB]) has (?<amount>.+)")]
        void accountX_has_Ym(Account account, decimal amount)
        {
            account.Balance = amount * 1m;
        }

        [DSL(@"the current user is authenticated for (?<account>Account[AB])")]
        void authenticate_for_account_x(Account account)
        {
            account.IsAuthenticated = true;
        }

        #endregion


        #region When

        [DSL(@"transfering (?<amount>.+) from (?<from>Account[AB]) to (?<to>Account[AB])")]
        void transfering_xm_from_a_to_b(decimal amount, Account from, Account to)
        {
            from.Transfer(amount * 1m, to);
        }
        #endregion

        #region Then

        [DSL(@"it should have (?<amount>.+) on (?<account>Account[AB])")]
        bool should_have_Xm_on_AccountY(Account account, decimal amount)
        {
            return account.Balance == amount * 1m;
        }

        [DSL]
        bool it_should_fail_with_error()
        {
           return WhenException != null;
        }

        #endregion

        #region Recursive DSL

        [DSL("(?<name>Account[AB])")]
        Account getaccountAB(string name)
        {
            return this.Get<Account>(name);
        }

        [DSL(@"(?<amount>\d+)m")]
        decimal getmillion(decimal amount)
        {
            return amount * 1m;
        }

        #endregion
    }
}
