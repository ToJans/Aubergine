using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aubergine.Tests.Accounts.SoftwareToTest.Model;

namespace Aubergine.Tests.Accounts.SoftwareToTest.Services
{
    public class AccountService
    {
        GenericRepository<User> _rUser;
        GenericRepository<Account> _rAccount;

        public AccountService(GenericRepository<User> rUser, GenericRepository<Account> rAccount)
        {
            _rUser = rUser;
            _rAccount = rAccount;
        }

        public ProcessStatus ValidateUser(string username, string password,ref User u)
        {
            u = GetUser(username,password);
            if (u == null)
                return new ProcessStatus() { Message = "Unknown username or password", Success = false };
            else
                return new ProcessStatus() { Message = "Welcome " + username, Success = true };
        }

        private User GetUser(string username, string password)
        {
            var u = _rUser.Find(username);
            if (u == null || u.Password != password)
                return null;
            else
                return u;
        }

        public ProcessStatus Transfer(User u,decimal amount, Account accFrom,Account accTo)
        {
            var msg = ValidateUser(u.Name, u.Password, ref u);
            if (msg.Success == false) return msg;

            if (accFrom == null)
                return new ProcessStatus() { Message = "Unknown account", Success = false }; 
            else if (accFrom.Owner !=u)
                return new ProcessStatus() { Message = "Access to this account is not allowed", Success = false };

            if (accTo == null)
                return new ProcessStatus() { Message = "Unknown account" , Success = false }; 
            
            if (accFrom.Balance < amount)
                return new ProcessStatus() { Message = "There is not enough money available on account "+accFrom.Number, Success = false };
            
            accFrom.Balance -= amount;
            accTo.Balance += amount;

            return new ProcessStatus() { Message = amount.ToString() + " was transferred from "+accFrom.Number + " to " + accTo.Number ,Success = true };
        }
    }
}
