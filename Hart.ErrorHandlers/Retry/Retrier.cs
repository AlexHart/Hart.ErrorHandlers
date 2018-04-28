using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Hart.ErrorHandlers.Retry
{

    /// <summary>
    /// Retry functions and actions in a safe way.
    /// </summary>
    public static class Retrier
    {
        /// <summary>
        /// Initialize the retrier with the default configuration.
        /// </summary>
        /// <returns></returns>
        public static RetryConfig Init() {
            return Init(0, 3);
        }

        /// <summary>
        /// Initialize the retrier.
        /// </summary>
        /// <param name="msWait"></param>
        /// <param name="maxRetries"></param>
        /// <returns></returns>
        public static RetryConfig Init(int msWait, int maxRetries) {
            return new RetryConfig()
            {
                MsWait = msWait,
                MaxRetries = maxRetries
            };
        }

        /// <summary>
        /// Initialize the retrier.
        /// </summary>
        /// <param name="msWait"></param>
        /// <param name="maxRetries"></param>
        public static RetryConfig Init(RetryConfig retryConfig) {           
            return retryConfig;
        }

        /// <summary>
        /// Retry a function n times or until successful.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="function"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private static RetryResult<T> retryFunc<T>(Func<T> function, RetryConfig config)
        {
			RetryInfo retryInfo = new RetryInfo();
            RetryResult<T> result = new RetryResult<T>(default(T), retryInfo);

            bool isOk = false;
            int retriesRemaining = config.MaxRetries;

            while (!isOk && (retriesRemaining >= 0 || config.RetryForever))
            {
                result.RetryInfo.Executions++;

                try
                {
                    result.Result = function.Invoke();
                    isOk = true;
                }
                catch (Exception ex)
                {
                    result.RetryInfo.Exceptions.Add(ex);

                    // Wait before retrying.
                    if (config.MsWait > 0)
                        Thread.Sleep(config.MsWait);
                }
				
                retriesRemaining--;
            }

            return result;
        }

        /// <summary>
        /// Retry a function n times or until successful asynchronously.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="function"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private static async Task<RetryResult<T>> retryFuncAsync<T>(Func<Task<T>> function, RetryConfig config)
        {
            RetryInfo retryInfo = new RetryInfo();
            RetryResult<T> result = new RetryResult<T>(default(T), retryInfo);

            bool isOk = false;
            int retriesRemaining = config.MaxRetries;

            while (!isOk && (retriesRemaining >= 0 || config.RetryForever))
            {
                result.RetryInfo.Executions++;

                try
                {
                    result.Result = await function.Invoke();
                    isOk = true;
                }
                catch (Exception ex)
                {
                    result.RetryInfo.Exceptions.Add(ex);

                    // Wait before retrying.
                    if (config.MsWait > 0)
                        Thread.Sleep(config.MsWait);
                }

                retriesRemaining--;
            }

            return result;
        }

        /// <summary>
        /// Retry an action n times or until successful.
        /// </summary>
        /// <param name="fun"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private static RetryResult retryAction(Action fun, RetryConfig config)
        {
            RetryInfo retryInfo = new RetryInfo();
            RetryResult result = new RetryResult(retryInfo);

            bool isOk = false;
            int retriesRemaining = config.MaxRetries;

            while (!isOk && (retriesRemaining >= 0 || config.RetryForever)) {
                result.RetryInfo.Executions++;

                try
                {
                    fun.Invoke();
                    isOk = true;
                }
                catch (Exception ex)
                {
                    result.RetryInfo.Exceptions.Add(ex);

                    // Wait before retrying.
                    if (config.MsWait > 0)
                        Thread.Sleep(config.MsWait);
                }

                retriesRemaining--;
            }

            return result;
        }

        /// <summary>
        /// Invoke the function that should be retried if fails.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="config"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        public static RetryResult<T> Invoke<T>(this RetryConfig config, Func<T> function)
        {
            if (function == null)
                throw new ArgumentNullException(nameof(function));

            return retryFunc(function, config);
        }

        /// <summary>
        /// Invoke asynchronously the function or action that should be retried if fails.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="config"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        public static async Task<RetryResult<T>> InvokeAsync<T>(this RetryConfig config, Func<Task<T>> function)
        {
            if (function == null)
                throw new ArgumentNullException(nameof(function));

            return await retryFuncAsync(function, config);
        }

        /// <summary>
        /// Invoke the action that should be retried if fails.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static RetryResult Invoke(this RetryConfig config, Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return retryAction(action, config);
        }

    }
}
