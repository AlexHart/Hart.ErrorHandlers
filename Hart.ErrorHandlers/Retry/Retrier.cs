﻿using System;
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
        /// Initialize the retrier.
        /// </summary>
        /// <param name="msWait"></param>
        /// <param name="maxRetries"></param>
        /// <returns></returns>
        public static RetryConfig Init(int msWait = 0, int maxRetries = 3) {
            return new RetryConfig()
            {
                MsWait = msWait,
                MaxRetries = maxRetries
            };
        }

        /// <summary>
        /// Initialize the retrier.
        /// </summary>
        /// <param name="retryConfig"></param>
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
        private static RetryResult<T> RetryFunc<T>(Func<T> function, RetryConfig config)
        {
			var retryInfo = new RetryInfo();
            var result = new RetryResult<T>(default(T), retryInfo);

            var isOk = false;
            var retriesRemaining = config.MaxRetries;

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
        private static async Task<RetryResult<T>> RetryFuncAsync<T>(Func<Task<T>> function, RetryConfig config)
        {
            var retryInfo = new RetryInfo();
            var result = new RetryResult<T>(default(T), retryInfo);

            var isOk = false;
            var retriesRemaining = config.MaxRetries;

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
        private static RetryResult RetryAction(Action fun, RetryConfig config)
        {
            var retryInfo = new RetryInfo();
            var result = new RetryResult(retryInfo);

            var isOk = false;
            var retriesRemaining = config.MaxRetries;

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

            return RetryFunc(function, config);
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

            return await RetryFuncAsync(function, config);
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

            return RetryAction(action, config);
        }

    }
}
