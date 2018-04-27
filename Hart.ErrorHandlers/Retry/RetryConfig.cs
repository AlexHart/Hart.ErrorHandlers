﻿using System;

namespace Hart.ErrorHandlers
{
    /// <summary>
    /// Retrier configuration.
    /// </summary>
    public class RetryConfig
    {

        /// <summary>
        /// Miliseconds to wait between requests.
        /// </summary>
        public int MsWait { get; set; } = 0;

        /// <summary>
        /// Maximum number of times to retry a failing function.
        /// </summary>
        /// <remarks>A number to high will cause a StackOverflowException</remarks>
        public int MaxRetries { get; set; } = 3;

    }
}
