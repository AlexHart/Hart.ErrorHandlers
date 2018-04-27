using System;
using System.Collections.Generic;
using System.Linq;

namespace Hart.ErrorHandlers
{
    public class RetryInfo
    {
        public int Executions { get; set; }

        public IList<Exception> Exceptions = new List<Exception>();

    }
}
