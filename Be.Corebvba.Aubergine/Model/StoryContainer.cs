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

        public Type ContextType {get;set;}

        public IElement Story { get; set; }

        public string ColumnToken { get; set; } 

        #endregion
    }
}
