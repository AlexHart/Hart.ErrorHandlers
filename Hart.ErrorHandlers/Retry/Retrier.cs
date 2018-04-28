using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.ComponentModel;

namespace Hart.ErrorHandlers.Retry
{

    //TODO: Add comments.
    //TODO: Make async version.

    public static class Retrier
    {
        public static RetryConfig Init() {
            return Init(0, 3);
        }

        public static RetryConfig Init(int msWait, int maxRetries) {
            return new RetryConfig()
            {
                MsWait = msWait,
                MaxRetries = maxRetries
            };
        }

        public static RetryConfig Init(RetryConfig retryConfig) {           
            return retryConfig;
        }

        private static RetryResult<T> retryFunc<T>(Func<T> fun, RetryConfig config)
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
                    result.Result = fun.Invoke();
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

        public static RetryResult<T> Invoke<T>(this RetryConfig config, Func<T> function)
        {
            if (function == null)
                throw new ArgumentNullException(nameof(function));

            return retryFunc(function, config);
        }

        public static RetryResult Invoke(this RetryConfig config, Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return retryAction(action, config);
        }

    }
}
