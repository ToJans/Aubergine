using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Corebvba.Aubergine.Examples.Accounts.SoftwareToTest.Model
{
    public class Account
    {
        public Account()
        {
            Balance = 0;
        }

        public User Owner { get; set; }

        public Decimal Balance { get; set; }

    }
}
