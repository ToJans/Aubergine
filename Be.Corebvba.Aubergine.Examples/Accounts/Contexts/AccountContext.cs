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

        [DSL(@"(Account[AB])_has_(\d+)_m")]
        void accountX_has_Ym(string x, string y)
        {
            this.Get<Account>(x).Balance = y.As<decimal>() * 1m;
        }

        [DSL(@"should_have_(\d+)_m_on_(Account[AB])")]
        void should_have_Xm_on_AccountY(string x, string y)
        {
            this.Get<Account>(y).Balance.ShouldEqual(x.As<decimal>() * 1m);
        }

        [DSL(@"transfering_(\d+)_m_from_(Account[AB])_to_(Account[AB])")]
        void transfering_xm_from_a_to_b(string x, string a, string b)
        {
            this.Get<Account>(a).Transfer(x.As<decimal>() * 1m, this.Get<Account>(b));
        }

        [DSL(@"the_current_user_is_authenticated_for_(Account[AB])")]
        void authenticate_for_account_x(string x)
        {
            this.Get<Account>(x).IsAuthenticated = true;
        }

        [DSL]
        void it_should_fail_with_error()
        {
          (WhenException != null).ShouldEqual(true);
        }
    }
}
