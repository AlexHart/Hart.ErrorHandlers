using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Hart.ErrorHandlers.Retry
{
    public static class RetrierExtensions
    {

        public static Retrier<T> WithMsWaitOf<T>(this Retrier<T> retryInfo, int msWait)
        {
            retryInfo.Config.MsWait = msWait;
            return retryInfo;
        }

        public static Retrier<T> WithNumberOfRetries<T>(this Retrier<T> retryInfo, int retries)
        {
            retryInfo.Config.MaxRetries = retries;
            return retryInfo;
        }

    }

    public class RetryConfig<T>
    {
        public int MsWait { get; set; } = 0;

        public int MaxRetries { get; set; } = 3;
    }

    public class Retrier<T> : IDisposable
    {
        private T retryFunc(Func<T> fun, int retries)
        {
            try
            {
                return fun.Invoke();
            }
            catch (Exception)
            {
                if (retries > 0)
                {
                    if (Config.MsWait > 0)
                        Thread.Sleep(Config.MsWait);

                    return retryFunc(fun, retries - 1);
                }
                else
                {
                    throw;
                }
            }
        }

        public RetryConfig<T> Config = new RetryConfig<T>();

        public T Invoke(Func<T> function)
        {
            try
            {
                if (function == null)
                    throw new ArgumentNullException(nameof(function));

                return retryFunc(function, Config.MaxRetries);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Dispose()
        {
            Config = null;
        }
    }
}
