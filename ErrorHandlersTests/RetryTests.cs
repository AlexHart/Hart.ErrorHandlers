using Hart.ErrorHandlers;
using System;
using Xunit;
using System.Linq;
using Hart.ErrorHandlers.Retry;
using Xunit.Sdk;

namespace ErrorHandlersTests
{

    public class RetrierTests
    {

        [Fact]
        public void RetryWithDefaultConfig()
        {
            // Arrange
            int res = 0;

            // Act.
            res = Retrier.Init<int>()
                         .Invoke(() => 2 + 2);

            // Assert.
            Assert.Equal(4, res);
        }

        [Fact]
        public void InitDefaultRetryConfig()
        {
            // Arrange
            // Act.
            var config = Retrier.Init<int>();

            // Assert.
            Assert.Equal(0, config.MsWait);
            Assert.Equal(3, config.MaxRetries);
            Assert.Null(config.OnFail);
        }

        [Fact]
        public void InitRetryConfig()
        {
            // Arrange
            Func<int> onFail = () => 2 + 2;

			// Act.
            var config = Retrier.Init<int>(200, 10, onFail);

            // Assert.
            Assert.Equal(200, config.MsWait);
            Assert.Equal(10, config.MaxRetries);
            Assert.Equal(onFail, config.OnFail);
        }

        [Fact]
        public void RetryFunctionOk()
        {
            // Arrange
            int res = 0;

            // Act.
            res = Retrier.Init<int>()
                         .WithMsWaitOf(200)
                         .WithNumberOfRetries(3)
                         .Invoke(() => 2 + 2);

            // Assert.
            Assert.Equal(4, res);
        }

        [Fact]
        public void RetryFunctionWithException()
        {
            // Arrange & Act.
            var ex = Record.Exception(() =>
            {
                int zero = 0;
                int res = Retrier.Init<int>()
                                 .WithMsWaitOf(0)
                                 .WithNumberOfRetries(1)
                                 .Invoke(() => 2 / zero);
            });

            // Assert.
            Assert.NotNull(ex);
            Assert.IsType<DivideByZeroException>(ex);
        }

        [Fact]
        public void RetryFunctionWithExceptionAndFailFallback()
        {
            // Arrange & Act.
            var ex = Record.Exception(() =>
            {
                int zero = 0;
                int res = Retrier.Init<int>()
                                 .WithMsWaitOf(0)
                                 .WithNumberOfRetries(1)
                                 .OnFail<int>(() => throw new ArgumentException("test"))
                                 .Invoke(() => 2 / zero);
            });

            // Assert.
            Assert.NotNull(ex);
            Assert.IsType<ArgumentException>(ex);
        }

        [Fact]
        public void RetryFunctionWithFallback()
        {
            // Arrange & Act.
            int zero = 0;

            int res = Retrier.Init<int>()
                             .WithMsWaitOf(0)
                             .WithNumberOfRetries(1)
                             .OnFail<int>(() => 123)
                             .Invoke(() => 2 / zero);

            // Assert.
            Assert.Equal(123, res);
        }

        [Fact]
        public void RetryFunctionWithFallbackReturnsSuccess()
        {
            // Arrange & Act.
            int zero = 0;

            var res = Retrier.Init<IResult>()
                             .WithMsWaitOf(0)
                             .WithNumberOfRetries(1)
                             .OnFail<IResult>(() => new Success())
                             .Invoke(() => {
                                 int val = 2 / zero;
                                 return new Success<int>(val);
                             });

            // Assert.
            Assert.True(res is Success);
        }

        [Fact]
        public void RetryFunctionWithFallbackReturnsError()
        {
            // Arrange & Act.
            int zero = 0;

            var exception = new ArgumentNullException();

            var res = Retrier.Init<IResult>()
                             .WithMsWaitOf(0)
                             .WithNumberOfRetries(1)
                             .OnFail<IResult>(() => new Error(exception))
                             .Invoke(() => {
                                 int val = 2 / zero;
                                 return new Success<int>(val);
                             });

            // Assert.
            Assert.True(res is Error);
            Assert.Equal(typeof(ArgumentNullException), (res as Error).Value.GetType());
        }

    }
}
