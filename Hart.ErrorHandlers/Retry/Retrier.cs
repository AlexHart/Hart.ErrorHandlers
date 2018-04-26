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

        private static T  retryFunc<T>(Func<T> fun, int retries, int msWait)
        {
            try
            {
                return fun.Invoke();
            }
            catch (Exception ex)
            {
                if (retries > 0)
                {
                    if (msWait > 0)
                        Thread.Sleep(msWait);

                    return retryFunc(fun, retries - 1, msWait);
                }
                else
                {
                    throw;
                }
            }
        }

        public static T Invoke<T>(this RetryConfig<T> config, Func<T> function)
        {
            T result;
            try
            {
                if (function == null)
                    throw new ArgumentNullException(nameof(function));

                T funcRes = retryFunc(function, config.MaxRetries, config.MsWait);
                result = funcRes;
            }
            catch (Exception ex)
            {
                if (config.OnFail != null)
                {
                    try
                    {
                        result = config.OnFail();
                    }
                    catch (Exception onFailEx)
                    {
                        throw onFailEx;
                    }
                } else {
                    throw ex;
                }
            }
            return result;
        }

        public static RetryResult<IResult> InvokeSafe<T>(this RetryConfig<T> config, Func<T> function)
        {
            RetryResult<IResult> res = new RetryResult<IResult>();
            try
            {
                if (function == null)
                    throw new ArgumentNullException(nameof(function));

                var funcRes = retryFunc(function, config.MaxRetries, config.MsWait);
                res.Result = new Success<T>(funcRes);
                res.Successful = true;
            }
            catch (Exception ex)
            {
                if (config.OnFail != null)
                {
                    try
                    {
                        var onFailRes = config.OnFail.Invoke();
                        res.Result = new Success<T>(onFailRes);
                        res.Successful = false;
                    }
                    catch (Exception onFailEx)
                    {
                        res.Result = new Error(onFailEx);
                    }
                }
                else 
                {
                    res.Result = new Error(ex);    
                }
				
                res.Successful = false;
            }
            return res;
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
