using Hart.ErrorHandlers;
using System;
using Xunit;
using System.Linq;
using Hart.ErrorHandlers.Retry;
using Xunit.Sdk;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ErrorHandlersTests.Retry
{

    public class RetrierTestsTimeout
    {

        [Fact]
        public void ExpireAfterTotalTimeoutWithNumberOfRetries()
        {
            // Arrange & Act.
            int zero = 0;
            RetryResult<int> result = Retrier.Init()
                                                .WithWaitOf(TimeSpan.FromMilliseconds(500))
                                                .WithNumberOfRetries(200)
                                                .TimeoutAfter(TimeSpan.FromSeconds(1))
                                                .Invoke(() => 2 / zero);
            // Assert.
            Assert.Equal(0, result.Result);
            Assert.False(result.Successful);
            Assert.Equal(2, result.RetryInfo.Executions);
            Assert.IsType<DivideByZeroException>(result.RetryInfo.Exceptions.FirstOrDefault());
        }

        [Fact]
        public void ExpireAfterTotalTimeoutWithRetryForever()
        {
            // Arrange & Act.
            int zero = 0;
            RetryResult<int> result = Retrier.Init()
                                                .WithWaitOf(TimeSpan.FromMilliseconds(500))
                                                .RetryUntilSuccessful()
                                                .TimeoutAfter(TimeSpan.FromSeconds(1))
                                                .Invoke(() => 2 / zero);
            // Assert.
            Assert.Equal(0, result.Result);
            Assert.False(result.Successful);
            Assert.Equal(2, result.RetryInfo.Executions);
            Assert.IsType<DivideByZeroException>(result.RetryInfo.Exceptions.FirstOrDefault());
        }
    }
}
