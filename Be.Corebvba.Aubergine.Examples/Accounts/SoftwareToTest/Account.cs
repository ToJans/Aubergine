using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Corebvba.Aubergine.Examples.Accounts.SoftwareToTest
{
    public class Account
    {
        public Account()
        {
            Balance = 0;
            IsAuthenticated = false;
        }

        public Decimal Balance { get; set; }

        public bool IsAuthenticated { get; set; }

        public void Transfer(decimal amount, Account ToAccount)
        {
            if (!IsAuthenticated)
                throw new UnauthorizedAccessException("You have no access to this account");

            if (Balance < amount)
            {
                throw new ArgumentOutOfRangeException("There is not enough money available on the account");
            }

            Balance -= amount;
            ToAccount.Balance += amount;
        }
    }
}
