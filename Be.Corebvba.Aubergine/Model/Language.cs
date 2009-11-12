using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Corebvba.Aubergine.Model
{
    public class Language
    {
        public string Story { get; set; }
        public string Scenario { get; set; }
        public string Given { get; set; }
        public string When { get; set; }
        public string Then { get; set; }
        public string And { get; set; }
        public string ColumnSeparator { get; set; }

        public Language() 
        { 
            Story = "Story";
            Scenario = "Scenario";
            Given = "Given";
            When = "When";
            Then = "Then";
            And = "Then";
            ColumnSeparator = "|";
        }

    }
}
