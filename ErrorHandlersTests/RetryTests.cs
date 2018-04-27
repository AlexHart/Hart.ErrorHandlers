using Hart.ErrorHandlers;
using System;
using Xunit;
using System.Linq;
using Hart.ErrorHandlers.Retry;
using Xunit.Sdk;
using System.Diagnostics;

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
                                                .WithFallBack(() => 1 + 1);
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
                             .WithFallBack(() => throw new ArgumentException("test"));

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
                             .WithFallBack(() => 2 * 2);

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

        [Fact]
        public void RetryActionSuccessful()
        {
            // Arrange & Act.
            var res = Retrier.Init()
                             .WithMsWaitOf(0)
                             .WithNumberOfRetries(1)
                             .Invoke(() => Trace.WriteLine("hello"));

            // Assert.
            Assert.True(res.Successful);
            Assert.False(res.ExecutedFallBack);
        }

        [Fact]
        public void RetryActionFailure()
        {
            // Arrange & Act.
            var res = Retrier.Init()
                             .WithMsWaitOf(0)
                             .WithNumberOfRetries(1)
                             .Invoke(() => {
                                 throw new NullReferenceException();
                             });

            // Assert.
            Assert.False(res.Successful);
            Assert.False(res.ExecutedFallBack);
            Assert.IsType<NullReferenceException>(res.RetryInfo.Exceptions.FirstOrDefault());
            Assert.Equal(2, res.RetryInfo.Executions);
        }

        [Fact]
        public void RetryActionFailureWithSuccessfulFallback()
        {
            // Arrange & Act.
            var res = Retrier.Init()
                             .WithMsWaitOf(0)
                             .WithNumberOfRetries(1)
                             .Invoke(() => {
                                 throw new NullReferenceException();
                             })
                             .WithFallBack(() => {
                                 // Empty action.
                             });

            // Assert.
            Assert.False(res.Successful);
            Assert.True(res.ExecutedFallBack);
            Assert.True(res.SuccessfulFallback);
            Assert.IsType<NullReferenceException>(res.RetryInfo.Exceptions.FirstOrDefault());
            Assert.Equal(2, res.RetryInfo.Executions);
        }

        [Fact]
        public void RetryActionFailureWithFailedFallback()
        {
            // Arrange & Act.
            var res = Retrier.Init()
                             .WithMsWaitOf(0)
                             .WithNumberOfRetries(1)
                             .Invoke(() => {
                                 throw new NullReferenceException();
                             })
                             .WithFallBack(() => {
                                 throw new DivideByZeroException();
                             });

            // Assert.
            Assert.False(res.Successful);
            Assert.True(res.ExecutedFallBack);
            Assert.False(res.SuccessfulFallback);
            Assert.IsType<DivideByZeroException>(res.FallBackException);
            Assert.IsType<NullReferenceException>(res.RetryInfo.Exceptions.FirstOrDefault());
            Assert.Equal(2, res.RetryInfo.Executions);
        }

        [Fact]
        public void RetryActionFailureWithZeroRetries()
        {
            // Arrange & Act.
            var res = Retrier.Init()
                             .WithNumberOfRetries(0)
                             .Invoke(() => {
                                 throw new NullReferenceException();
                             });

            // Assert.
            Assert.False(res.Successful);
            Assert.Equal(1, res.RetryInfo.Executions);
        }

        [Fact]
        public void RetryFunctionFailureWithZeroRetries()
        {
            // Arrange & Act.
            var res = Retrier.Init()
                             .WithNumberOfRetries(0)
                             .Invoke<int>(() => {
                                 throw new NullReferenceException();
                             });

            // Assert.
            Assert.False(res.Successful);
            Assert.Equal(1, res.RetryInfo.Executions);
        }

        [Fact]
        public void RetryForeverConfigTrue() {
            // Arrange & Act.
            var config = Retrier.Init()
                             .WithMsWaitOf(0)
                             .RetryUntilSuccessful();

            // Assert.
            Assert.True(config.RetryForever);
        }

        [Fact]
        public void RetryForeverConfigFalse()
        {
            // Arrange & Act.
            var config = Retrier.Init();

            // Assert.
            Assert.False(config.RetryForever);
        }

        [Fact]
        public void RetryForeverTrue()
        {
            // Arrange.
            int i = 0;
            int zero = 0;
            int expectedReturn = 1101;
            int iterations = 10;

            Func<int> func = () =>
            {
                i += 1;
                if (i == iterations)
                    return expectedReturn;
                else
                    return i / zero;
            };

            // Act.
            var result = Retrier.Init()
                                .WithMsWaitOf(0)
                                .RetryUntilSuccessful()
                                .Invoke(func);

            // Assert.
            Assert.Equal(expectedReturn, result.Result);
            Assert.Equal(iterations, result.RetryInfo.Executions);
            Assert.IsType<DivideByZeroException>(result.RetryInfo.Exceptions.FirstOrDefault());
            Assert.Equal(iterations - 1, result.RetryInfo.Exceptions.Count());
            Assert.False(result.Successful);
        }

    }
}
