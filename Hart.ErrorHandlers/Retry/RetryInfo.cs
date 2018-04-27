using System;
using System.Collections.Generic;
using System.Linq;

namespace Hart.ErrorHandlers
{

    /// <summary>
    /// Retry execution information.
    /// </summary>
    public class RetryInfo
    {
        /// <summary>
        /// Number of times the method was called.
        /// </summary>
        public int Executions { get; set; }

        /// <summary>
        /// List with all the exceptions throwed during executions.
        /// </summary>
        public IList<Exception> Exceptions = new List<Exception>();

    }
}
