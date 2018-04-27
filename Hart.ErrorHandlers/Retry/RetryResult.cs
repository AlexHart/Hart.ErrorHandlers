using System;
using System.Collections.Generic;
using System.Text;

namespace Hart.ErrorHandlers.Retry
{

    /// <summary>
    /// Holds the information of a function Invoke inside Retrier.
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    public class RetryResult<T>
    {
        /// <summary>
        /// Result of the function.
        /// </summary>
        /// <remarks>If failed will return the default value of T</remarks>
        public T Result { get; set; }

        /// <summary>
        /// Information about retrying operation.
        /// </summary>
        public RetryInfo RetryInfo { get; set; }

        /// <summary>
        /// Returns if the operation was successful.
        /// </summary>
        public bool Successful => RetryInfo.Exceptions.Count == 0 && FallBackException == null;

        /// <summary>
        /// Returns if the fallback had to be executed.
        /// </summary>
        public bool ExecutedFallBack { get; set; }

        /// <summary>
        /// Exception throwed in the fallback if it failed too.
        /// </summary>
        public Exception FallBackException { get; set; }

        /// <summary>
        /// Returns if the fallback was successful.
        /// </summary>
        public bool SuccessfulFallback => FallBackException == null;

        /// <summary>
        /// Configure RetryResult with the minimum information.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="retryInfo"></param>
        public RetryResult(T result, RetryInfo retryInfo) {
            Result = result;
            RetryInfo = retryInfo;
        }
    }


    /// <summary>
    /// Holds the information of an Action Invoke inside Retrier.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RetryResult
    {
        /// <summary>
        /// Information about retrying operation.
        /// </summary>
        public RetryInfo RetryInfo { get; set; }

        /// <summary>
        /// Returns if the operation was successful.
        /// </summary>
        public bool Successful => RetryInfo.Exceptions.Count == 0 && FallBackException == null;

        /// <summary>
        /// Returns if the fallback had to be executed.
        /// </summary>
        public bool ExecutedFallBack { get; set; }

        /// <summary>
        /// Exception throwed in the fallback if it failed too.
        /// </summary>
        public Exception FallBackException { get; set; }

        /// <summary>
        /// Returns if the fallback was successful.
        /// </summary>
        public bool SuccessfulFallback => FallBackException == null;

        /// <summary>
        /// Configure RetryResult with the minimum information.
        /// </summary>
        /// <param name="retryInfo"></param>
        public RetryResult(RetryInfo retryInfo)
        {
            RetryInfo = retryInfo;
        }
    }
}
