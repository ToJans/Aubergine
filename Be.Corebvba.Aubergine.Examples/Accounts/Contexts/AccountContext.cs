using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Be.Corebvba.Aubergine.Examples.Accounts.SoftwareToTest;

namespace Be.Corebvba.Aubergine.Examples.Accounts.Contexts
{
    internal class AccountContext
    {
        public Account AccountA = new Account();
        public Account AccountB = new Account();
        public Exception WhenException;

        [DSL(@"(?<account>Account[AB])_has_(?<amount>\d+)_m")]
        void accountX_has_Ym(Account account, decimal amount)
        {
            account.Balance = amount * 1m;
        }

        [DSL(@"it_should_have_(?<amount>\d+)_m_on_(?<account>Account[AB])")]
        void should_have_Xm_on_AccountY(Account account, decimal amount)
        {
            account.Balance.ShouldEqual(amount * 1m);
        }

        [DSL(@"transfering_(?<amount>\d+)_m_from_(?<from>Account[AB])_to_(?<to>Account[AB])")]
        void transfering_xm_from_a_to_b(decimal amount, Account from, Account to)
        {
            from.Transfer(amount * 1m, to);
        }

        [DSL(@"the_current_user_is_authenticated_for_(?<account>Account[AB])")]
        void authenticate_for_account_x(Account account)
        {
            account.IsAuthenticated = true;
        }

        [DSL]
        void it_should_fail_with_error()
        {
          (WhenException != null).ShouldEqual(true);
        }

        [DSL("(?<name>Account[AB])")]
        Account getaccountAB(string name)
        {
            return this.Get<Account>(name);
        }
    }
}
