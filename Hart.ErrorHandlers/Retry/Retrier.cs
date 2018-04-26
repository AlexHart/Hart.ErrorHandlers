using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.ComponentModel;

namespace Hart.ErrorHandlers.Retry
{

    public static class Retrier
    {
        public static RetryConfig<T> Init<T>() {
            return Init<T>(0, 3, null);
        }

        public static RetryConfig<T> Init<T>(int msWait, int maxRetries, Func<T> onFail = null) {
            return new RetryConfig<T>()
            {
                MsWait = msWait,
                MaxRetries = maxRetries,
                OnFail = onFail
            };
        }

        public static RetryConfig<T> Init<T>(RetryConfig<T> retryConfig) {
            if (retryConfig.OnFail == null)
                throw new ArgumentNullException(nameof(retryConfig));
            
            return retryConfig;
        }

        private static T retryFunc<T>(Func<T> fun, int retries, int msWait, Func<T> onFail)
        {
            try
            {
                return fun.Invoke();
            }
            catch (Exception)
            {
                if (retries > 0)
                {
                    if (msWait > 0)
                        Thread.Sleep(msWait);

                    return retryFunc(fun, retries - 1, msWait, onFail);
                }
                else
                {
                    if (onFail != null) 
                    {
                        try
                        {
                            return onFail.Invoke();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }  
                    }else{
                        throw;
                    }
                }
            }
        }

        public static T Invoke<T>(this RetryConfig<T> config, Func<T> function)
        {
            try
            {
                if (function == null)
                    throw new ArgumentNullException(nameof(function));

                return retryFunc(function, config.MaxRetries, config.MsWait, config.OnFail);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static RetryConfig<T> WithMsWaitOf<T>(this RetryConfig<T> retryConfig, int msWait)
        {
            retryConfig.MsWait = msWait;
            return retryConfig;
        }

        public static RetryConfig<T> WithNumberOfRetries<T>(this RetryConfig<T> retryConfig, int retries)
        {
            retryConfig.MaxRetries = retries;
            return retryConfig;
        }

        public static RetryConfig<T> OnFail<T>(this RetryConfig<T> retryConfig, Func<T> onFail)
        {
            if (onFail == null)
                throw new ArgumentNullException(nameof(onFail));
                
            retryConfig.OnFail = onFail;
            return retryConfig;
        }

    }
}
