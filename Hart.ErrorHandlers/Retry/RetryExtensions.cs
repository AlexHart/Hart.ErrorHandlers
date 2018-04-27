using System;
using System.Collections.Generic;
using System.Text;

namespace Hart.ErrorHandlers.Retry
{
    public static class RetryExtensions
    {

        public static RetryConfig WithMsWaitOf(this RetryConfig retryConfig, int msWait)
        {
            retryConfig.MsWait = msWait;
            return retryConfig;
        }

        public static RetryConfig WithNumberOfRetries(this RetryConfig retryConfig, int retries)
        {
            retryConfig.MaxRetries = retries;
            return retryConfig;
        }

        public static RetryResult<T> WithFallBackFunction<T>(this RetryResult<T> result, Func<T> fallback)
        {
            if (fallback == null)
                throw new ArgumentNullException(nameof(fallback));

            if (!result.Successful)
            {
                try
                {
                    result.ExecutedFallBack = true;
                    result.Result = fallback.Invoke();
                }
                catch (Exception ex)
                {
                    result.FallBackException = ex;
                }
            }

            return result;
        }

        public static RetryResult<T> WithFallBackValue<T>(this RetryResult<T> result, T defaultValue)
        {
            if (defaultValue == null)
                throw new ArgumentNullException(nameof(defaultValue));

            if (!result.Successful)
            {
                try
                {
                    result.ExecutedFallBack = true;
                    result.Result = defaultValue;
                }
                catch (Exception ex)
                {
                    result.FallBackException = ex;
                }
            }

            return result;
        }

    }
}
