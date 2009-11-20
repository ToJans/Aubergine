﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Corebvba.Aubergine.Interfaces
{
    public interface IStoryContainer
    {
        ISpecElement Story { get; set; }
        string ColumnToken { get; set; }
        string ContextName { get; set; }
    }
}
