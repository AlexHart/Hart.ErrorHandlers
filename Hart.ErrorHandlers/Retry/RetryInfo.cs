﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Hart.ErrorHandlers
{
    public class RetryInfo
    {
        public bool Successful => Exceptions.Count() == 0;

        public int Executions { get; set; }

        public IList<Exception> Exceptions = new List<Exception>();

    }
}