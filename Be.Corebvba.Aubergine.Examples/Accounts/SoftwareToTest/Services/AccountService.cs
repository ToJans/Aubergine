using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Be.Corebvba.Aubergine.Examples.Accounts.SoftwareToTest.Model;

namespace Be.Corebvba.Aubergine.Examples.Accounts.SoftwareToTest.Services
{
    public class AccountService
    {
        public ProcessStatus AuthenticateUser(User user)
        {
            if (user.Name == "Neo" && user.Password == "Red pill")
                return new ProcessStatus() { Message = "Welcome " + user.Name, Success = true };
            else
                return new ProcessStatus() { Message = "Wrong username or password", Success = false };

        }

        public ProcessStatus AuthenticateUserForAccount(Account a, User u)
        {
            var x = AuthenticateUser(u);
            if (!x.Success) return x;
            if (a.Owner == u)
                return new ProcessStatus() { Success = true };
            else
                return new ProcessStatus() { Message ="Access to this account is not allowed", Success = false };

        }


        public ProcessStatus Transfer(User u,decimal amount, Account From,Account ToAccount)
        {
            var x = AuthenticateUserForAccount(From,u);
            if (!x.Success) return x;

            if (From.Balance < amount)
                return new ProcessStatus() { Message = "There is not enough money available on the account", Success = false };
            
            From.Balance -= amount;
            ToAccount.Balance += amount;

            return new ProcessStatus() { Message = amount.ToString() + " was transferred",Success = true };
        }
    }
}
