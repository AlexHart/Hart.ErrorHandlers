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
            res = Retrier.Init()
                         .Invoke(() => 2 + 2)
                         .Result;

            // Assert.
            Assert.Equal(4, res);
        }

        [Fact]
        public void SuccessfulRetriesEqualZero()
        {
            // Arrange

            // Act.
            var res = Retrier.Init()
                         .Invoke(() => 2 + 2);

            // Assert.
            Assert.Equal(1, res.RetryInfo.Executions);
            Assert.Equal(4, res.Result);
            Assert.True(res.Successful);
            Assert.Empty(res.RetryInfo.Exceptions);
        }

        [Fact]
        public void InitDefaultRetryConfig()
        {
            // Arrange
            // Act.
            var config = Retrier.Init();

            // Assert.
            Assert.Equal(0, config.MsWait);
            Assert.Equal(3, config.MaxRetries);
        }

        [Fact]
        public void InitRetryConfig()
        {
            // Arrange && Act.
            var config = Retrier.Init(200, 10);

            // Assert.
            Assert.Equal(200, config.MsWait);
            Assert.Equal(10, config.MaxRetries);
        }

        [Fact]
        public void InitRetryConfigFluent()
        {
            // Arrange && Act.
            var config = Retrier.Init()
                                .WithNumberOfRetries(10)
                                .WithMsWaitOf(1000);

            // Assert.
            Assert.Equal(10, config.MaxRetries);
            Assert.Equal(1000, config.MsWait);
        }

        [Fact]
        public void InitRetryConfigDirectly()
        {
            // Arrange.
            var retryConfig = new RetryConfig()
            {
                MaxRetries = 10,
                MsWait = 1000
            };

            // Act.
            var config = Retrier.Init(retryConfig);

            // Assert.
            Assert.Equal(10, config.MaxRetries);
            Assert.Equal(1000, config.MsWait);
        }

        [Fact]
        public void CountExceptionsAndRetries()
        {
            // Arrange & Act.
            int zero = 0;
            RetryResult<int> result  = Retrier.Init()
                                                .WithMsWaitOf(0)
                                                .WithNumberOfRetries(2)
                                                .Invoke(() => 2 / zero);
            // Assert.
            Assert.Equal(0, result.Result);
            Assert.False(result.Successful);
            Assert.Equal(3, result.RetryInfo.Executions);
            Assert.IsType<DivideByZeroException>(result.RetryInfo.Exceptions.FirstOrDefault());
        }

        [Fact]
        public void SuccessfulFunctionDoesntGoToFallbackFunction()
        {
            // Arrange & Act.
            RetryResult<int> result = Retrier.Init()
                                                .WithMsWaitOf(0)
                                                .WithNumberOfRetries(2)
                                                .Invoke(() => 10 / 2)
                                                .WithFallBackFunction(() => 1 + 1);
            // Assert.
            Assert.Equal(5, result.Result);
            Assert.True(result.Successful);
            Assert.Equal(1, result.RetryInfo.Executions);
        }

        [Fact]
        public void SuccessfulFunctionDoesntReturnFallbackValue()
        {
            // Arrange & Act.
            RetryResult<int> result = Retrier.Init()
                                                .WithMsWaitOf(0)
                                                .WithNumberOfRetries(2)
                                                .Invoke(() => 10 / 2)
                                                .WithFallBackValue(1000);
            // Assert.
            Assert.Equal(5, result.Result);
            Assert.True(result.Successful);
            Assert.Equal(1, result.RetryInfo.Executions);
        }

        [Fact]
        public void RetryFunctionWithExceptionAndFailFallback()
        {
            // Arrange & Act.
            int zero = 0;
            var res = Retrier.Init()
                             .WithMsWaitOf(0)
                             .WithNumberOfRetries(1)
                             .Invoke(() => 2 / zero)
                             .WithFallBackFunction(() => throw new ArgumentException("test"));

            // Assert.
            Assert.Equal(0, res.Result);
            Assert.False(res.Successful);
            Assert.False(res.SuccessfulFallback);
            Assert.IsType<DivideByZeroException>(res.RetryInfo.Exceptions.FirstOrDefault());
            Assert.IsType<ArgumentException>(res.FallBackException);
        }

        [Fact]
        public void RetryFunctionWithExceptionAndSuccessfulFallbackFunction()
        {
            // Arrange & Act.
            int zero = 0;
            var res = Retrier.Init()
                             .WithMsWaitOf(0)
                             .WithNumberOfRetries(1)
                             .Invoke(() => 2 / zero)
                             .WithFallBackFunction(() => 2 * 2);

            // Assert.
            Assert.Equal(4, res.Result);
            Assert.False(res.Successful);
            Assert.True(res.SuccessfulFallback);
            Assert.IsType<DivideByZeroException>(res.RetryInfo.Exceptions.FirstOrDefault());
            Assert.Null(res.FallBackException);
        }

        [Fact]
        public void RetryFunctionWithExceptionAndSuccessfulFallbackValue()
        {
            // Arrange & Act.
            int zero = 0;
            var res = Retrier.Init()
                             .WithMsWaitOf(0)
                             .WithNumberOfRetries(1)
                             .Invoke(() => 2 / zero)
                             .WithFallBackValue(33);

            // Assert.
            Assert.Equal(33, res.Result);
            Assert.False(res.Successful);
            Assert.True(res.SuccessfulFallback);
            Assert.IsType<DivideByZeroException>(res.RetryInfo.Exceptions.FirstOrDefault());
            Assert.Null(res.FallBackException);
        }

    }
}
