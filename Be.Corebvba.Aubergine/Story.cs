using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Be.Corebvba.Aubergine
{
    public abstract class Story<T> {
        public class As_a {}
        public class As_an { }
        public class I_want { }
        public class So_that {}
        public delegate void Given(T context);
        public class Scenario
        {
            public delegate void Given(T context);
            public delegate void When(T context);
            public delegate void Then(T context);

        }
    
    }
}
