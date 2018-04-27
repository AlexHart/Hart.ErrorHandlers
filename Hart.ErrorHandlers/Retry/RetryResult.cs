using System;
using System.Collections.Generic;
using System.Text;

namespace Hart.ErrorHandlers.Retry
{
    public class RetryResult<T>
    {

        public T Result { get; private set; }

        public RetryInfo RetryInfo { get; private set; }

        public bool Successful => RetryInfo.Successful;

        public RetryResult(T result, RetryInfo retryInfo) {
            Result = result;
            RetryInfo = retryInfo;
        }

    }
}
