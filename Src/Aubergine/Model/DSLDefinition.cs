using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aubergine.Interfaces;

namespace Aubergine.Model
{
    public class DSLDefinition : IDSLDefinition
    {
        #region IDSLDefinition Members

        public Type ContextType { get; set; }

        public IEnumerable<IDSLElement> Elements {get;set;}

        public string Name { get; set; }

        #endregion
    }
}
