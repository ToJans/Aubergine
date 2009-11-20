using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Corebvba.Aubergine.Examples.Accounts.SoftwareToTest.Services
{
    public class GenericRepository<T> : List<T>
    {
        Func<T,string, bool> _FindbyName;
        public GenericRepository(Func<T,string, bool> findByName)
        {
            _FindbyName = findByName;
        }

        public T Find(string name)
        {
            return this.Where(o=>_FindbyName(o,name)).FirstOrDefault();
        }
    }
}
