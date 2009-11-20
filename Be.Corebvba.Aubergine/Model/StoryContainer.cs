using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Be.Corebvba.Aubergine.Interfaces;

namespace Be.Corebvba.Aubergine.Model
{
    public class StoryContainer : IStoryContainer 
    {

        #region IStoryContainer Members

        public ISpecElement Story { get; set; }

        public string ColumnToken { get; set; }

        public string ContextName { get; set; }

        #endregion
    }
}
