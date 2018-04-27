using System;
using System.Collections.Generic;
using System.Text;

namespace Hart.ErrorHandlers.Retry
{
    public class RetryResult<T>
    {

        public T Result { get; set; }

        public RetryInfo RetryInfo { get; set; }

        public bool Successful => RetryInfo.Exceptions.Count == 0 && FallBackException == null;

        public bool ExecutedFallBack { get; set; }

        public Exception FallBackException { get; set; }

        public bool SuccessfulFallback => FallBackException == null;

        public RetryResult(T result, RetryInfo retryInfo) {
            Result = result;
            RetryInfo = retryInfo;
        }

    }
}
