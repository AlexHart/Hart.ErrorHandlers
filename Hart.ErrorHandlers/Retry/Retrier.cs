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

        private static RetryResult<T> retryFunc<T>(Func<T> fun, int retries, int msWait, RetryInfo retryInfo)
        {
            retryInfo.Executions++;

            try
            {
                var funcRes = fun.Invoke();
                return new RetryResult<T>(funcRes, retryInfo);
            }
            catch (Exception ex)
            {
                retryInfo.Exceptions.Add(ex);

                if (retries == 0)
                {
                    return new RetryResult<T>(default(T), retryInfo);
                }
                else
                { 
                    if (msWait > 0)
                        Thread.Sleep(msWait);

                    return retryFunc(fun, retries - 1, msWait, retryInfo);
                }
            }
        }

        private static RetryResult retryAction(Action fun, int retries, int msWait, RetryInfo retryInfo)
        {
            retryInfo.Executions++;

            try
            {
                fun.Invoke();
                return new RetryResult(retryInfo);
            }
            catch (Exception ex)
            {
                retryInfo.Exceptions.Add(ex);

                if (retries == 0)
                {
                    return new RetryResult(retryInfo);
                }
                else
                {
                    if (msWait > 0)
                        Thread.Sleep(msWait);

                    return retryAction(fun, retries - 1, msWait, retryInfo);
                }
            }
        }

        public static RetryResult<T> Invoke<T>(this RetryConfig config, Func<T> function)
        {
            if (function == null)
                throw new ArgumentNullException(nameof(function));

            return retryFunc(function, config.MaxRetries, config.MsWait, new RetryInfo());
        }

        public static RetryResult Invoke(this RetryConfig config, Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return retryAction(action, config.MaxRetries, config.MsWait, new RetryInfo());
        }

    }
}
