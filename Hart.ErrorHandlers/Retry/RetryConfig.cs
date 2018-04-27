using System;

namespace Hart.ErrorHandlers
{
    public class RetryConfig
    {
        public int MsWait { get; set; } = 0;

        public int MaxRetries { get; set; } = 3;

    }
}
