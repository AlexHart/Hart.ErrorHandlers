using System;

namespace Hart.ErrorHandlers
{
    public class RetryConfig<T>
    {
        public int MsWait { get; set; } = 0;

        public int MaxRetries { get; set; } = 3;

        public Func<T> OnFail { get; set; }

    }
}
