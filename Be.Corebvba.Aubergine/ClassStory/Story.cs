using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Be.Corebvba.Aubergine.ClassStory
{
    public abstract class Story<T> {
        public class As_a {}
        public class As_an { }
        public class I_want { }
        public class So_that {}
        public class Given { }
        public class Scenario
        {
            public class Given { }
            public class When { }
            public class Then { }
        }
    
    }
}
