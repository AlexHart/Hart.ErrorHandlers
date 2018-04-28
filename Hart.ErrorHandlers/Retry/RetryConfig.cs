using System;

namespace Hart.ErrorHandlers.Retry
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

        /// <summary>
        /// Keep retrying indefenitely.
        /// </summary>
        /// <value><c>true</c> if retry forever; otherwise, <c>false</c>.</value>
        public bool RetryForever { get; set; } = false;
    }
}
