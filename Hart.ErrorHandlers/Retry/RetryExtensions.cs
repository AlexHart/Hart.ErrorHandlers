using System;
using System.Threading.Tasks;

namespace Hart.ErrorHandlers.Retry
{

    /// <summary>
    /// Extensions for retrying functionality.
    /// </summary>
    public static class RetryExtensions
    {

        /// <summary>
        /// Wait command n milliseconds between retries.
        /// </summary>
        /// <param name="retryConfig"></param>
        /// <param name="msWait"></param>
        /// <returns></returns>
        public static RetryConfig WithMsWaitOf(this RetryConfig retryConfig, int msWait)
        {
            retryConfig.WaitBetweenRetries = TimeSpan.FromMilliseconds(msWait);
            return retryConfig;
        }

        /// <summary>
        /// Wait command for an specified timespan duration between retries.
        /// </summary>
        /// <param name="retryConfig"></param>
        /// <param name="wait"></param>
        /// <returns></returns>
        public static RetryConfig WithWaitOf(this RetryConfig retryConfig, TimeSpan wait)
        {
            retryConfig.WaitBetweenRetries = wait;
            return retryConfig;
        }

        /// <summary>
        /// Specify the number of retries.
        /// </summary>
        /// <param name="retryConfig"></param>
        /// <param name="retries"></param>
        /// <returns></returns>
        public static RetryConfig WithNumberOfRetries(this RetryConfig retryConfig, int retries)
        {
            retryConfig.MaxRetries = retries;
            return retryConfig;
        }

        /// <summary>
        /// Retry until it's succesful.
        /// </summary>
        /// <param name="retryConfig"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Configure a fallback function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <param name="fallback"></param>
        /// <returns></returns>
        public static RetryResult<T> WithFallBack<T>(this RetryResult<T> result, Func<T> fallback)
        {
            if (fallback == null)
                throw new ArgumentNullException(nameof(fallback));

            if (!result.Successful)
            {
                try
                {
                    result.ExecutedFallBack = true;
                    result.Result = fallback();
                }
                catch (Exception ex)
                {
                    result.FallBackException = ex;
                }
            }

            return result;
        }

        /// <summary>
        /// Configure an async fallback function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <param name="fallback"></param>
        /// <returns></returns>
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

                    result.Result = await fallback();
                }
                catch (Exception ex)
                {
                    result.FallBackException = ex;
                }
            }

            return result;
        }

        /// <summary>
        /// Helper to extract result from a task.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        public static RetryResult<T> WaitForValue<T>(this Task<RetryResult<T>> result)
        {
            return result.Result;
        }

        /// <summary>
        /// Specify a fallback value if all the tries fail.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Configure a fallback action.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="fallback"></param>
        /// <returns></returns>
        public static RetryResult WithFallBack(this RetryResult result, Action fallback)
        {
            if (fallback == null)
                throw new ArgumentNullException(nameof(fallback));

            if (!result.Successful)
            {
                try
                {
                    result.ExecutedFallBack = true;
                    fallback();
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
