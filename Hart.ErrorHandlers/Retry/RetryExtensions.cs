﻿using System;
using System.Threading.Tasks;

namespace Hart.ErrorHandlers.Retry
{

    //TODO: Add comments.

    /// <summary>
    /// Extensions for retrying functionality.
    /// </summary>
    public static class RetryExtensions
    {

        public static RetryConfig WithMsWaitOf(this RetryConfig retryConfig, int msWait)
        {
            retryConfig.WaitBetweenRetries = TimeSpan.FromMilliseconds(msWait);
            return retryConfig;
        }

        public static RetryConfig WithWaitOf(this RetryConfig retryConfig, TimeSpan wait)
        {
            retryConfig.WaitBetweenRetries = wait;
            return retryConfig;
        }

        public static RetryConfig WithNumberOfRetries(this RetryConfig retryConfig, int retries)
        {
            retryConfig.MaxRetries = retries;
            return retryConfig;
        }

        public static RetryConfig RetryUntilSuccessful(this RetryConfig retryConfig) {
            retryConfig.RetryForever = true;
            return retryConfig;
        }

        /// <summary>
        /// Configure a maximum retry time.
        /// </summary>
        /// <returns>The after.</returns>
        /// <param name="retryConfig">Retry config.</param>
        /// <param name="timeout">Timeout.</param>
        public static RetryConfig TimeoutAfter(this RetryConfig retryConfig, TimeSpan timeout) {
            retryConfig.TotalTimeout = timeout;
            return retryConfig;
        }

        public static RetryResult<T> WithFallBack<T>(this RetryResult<T> result, Func<T> fallback)
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

        public static async Task<RetryResult<T>> WithFallBackAsync<T>(this RetryResult<T> result, Func<Task<T>> fallback)
        {
            if (fallback == null)
                throw new ArgumentNullException(nameof(fallback));

            if (!result.Successful)
            {
                try
                {
                    result.ExecutedFallBack = true;
                    result.RetryInfo.Executions++;

                    result.Result = await fallback.Invoke();
                }
                catch (Exception ex)
                {
                    result.FallBackException = ex;
                }
            }

            return result;
        }

        public static RetryResult<T> WaitForValue<T>(this Task<RetryResult<T>> result)
        {
            return result.Result;
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

        public static RetryResult WithFallBack(this RetryResult result, Action fallback)
        {
            if (fallback == null)
                throw new ArgumentNullException(nameof(fallback));

            if (!result.Successful)
            {
                try
                {
                    result.ExecutedFallBack = true;
                    fallback.Invoke();
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
