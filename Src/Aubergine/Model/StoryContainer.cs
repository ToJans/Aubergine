using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aubergine.Interfaces;

namespace Aubergine.Model
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
