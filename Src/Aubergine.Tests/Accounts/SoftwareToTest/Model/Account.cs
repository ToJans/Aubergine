using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aubergine.Tests.Accounts.SoftwareToTest.Model
{
    public class Account
    {
        public string Number { get; set; }

        public Account()
        {
            Balance = 0;
        }

        public User Owner { get; set; }

        public Decimal Balance { get; set; }

    }
}
