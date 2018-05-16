using Hart.ErrorHandlers;
using System;
using Xunit;
using System.Linq;
using Hart.ErrorHandlers.Retry;
using Xunit.Sdk;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ErrorHandlersTests
{

    public class RetrierTests
    {

        [Fact]
        public void RetryWithInvokeDirectly()
        {
            // Arrange
            var config = new RetryConfig()
            {
                MaxRetries = 10,
                WaitBetweenRetries = TimeSpan.FromSeconds(1),
                RetryForever = false
            };

            Func<int> fun = () => 2 + 2;

            // Act.
            var res = Retrier.Invoke(config, fun);

            // Assert.
            Assert.Equal(4, res.Result);
            Assert.Equal(1, res.RetryInfo.Executions);
            Assert.True(res.Successful);
        }

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
            Assert.Equal(0, config.WaitBetweenRetries.Milliseconds);
            Assert.Equal(3, config.MaxRetries);
        }

        [Fact]
        public void InitRetryConfig()
        {
            // Arrange && Act.
            var config = Retrier.Init(TimeSpan.FromMilliseconds(200), 10);

            // Assert.
            Assert.Equal(200, config.WaitBetweenRetries.Milliseconds);
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
            Assert.Equal(1, config.WaitBetweenRetries.Seconds);
        }

        [Fact]
        public void InitRetryConfigDirectly()
        {
            // Arrange.
            var retryConfig = new RetryConfig()
            {
                MaxRetries = 10,
                WaitBetweenRetries = TimeSpan.FromSeconds(1)
            };

            // Act.
            var config = Retrier.Init(retryConfig);

            // Assert.
            Assert.Equal(10, config.MaxRetries);
            Assert.Equal(1, config.WaitBetweenRetries.Seconds);
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
        public void RetryForeverFunctionTrue()
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

        [Fact]
        public void RetryForeverActionTrue()
        {
            // Arrange.
            int i = 0;
            int zero = 0;
            int iterations = 10;

            Action func = () =>
            {
                i += 1;
                if (i != iterations)
                {
                    int x = i / zero;
                }
            };

            // Act.
            var result = Retrier.Init()
                                .WithMsWaitOf(0)
                                .RetryUntilSuccessful()
                                .Invoke(func);

            // Assert.
            Assert.Equal(iterations, result.RetryInfo.Executions);
            Assert.IsType<DivideByZeroException>(result.RetryInfo.Exceptions.FirstOrDefault());
            Assert.Equal(iterations - 1, result.RetryInfo.Exceptions.Count());
            Assert.False(result.Successful);
        }

        [Fact]
        public void RetryAsyncFunction()
        {
            // Arrange && Act.
            var result = Retrier.Init()
                                .Invoke(async () => await FakeService.GetHelloWorldAsync());

            // Assert.
            Assert.Equal("Hello world", result.Result.Result);
            Assert.Equal(1, result.RetryInfo.Executions);
            Assert.True(result.Successful);
        }

        [Fact]
        public void RetryAsyncAction()
        {
            // Arrange && Act.
            var result = Retrier.Init()
                                .Invoke(async () => await FakeService.DoFakeCalculationsAsync());

            // Assert.
            Assert.Equal(1, result.RetryInfo.Executions);
            Assert.True(result.Successful);
        }

        [Fact]
        public async void RetryAsyncFailFunctionWithRegularAwait()
        {
            // Arrange & Act.
            var result = await Retrier.Init()
                                .WithNumberOfRetries(1)
                                .InvokeAsync(FakeService.DivideByZeroExceptionAsync);

            // Assert.
            Assert.False(result.Successful);
            Assert.IsType<DivideByZeroException>(result.RetryInfo.Exceptions.FirstOrDefault());
            Assert.Equal(2, result.RetryInfo.Executions);
        }

        [Fact]
        public void RetryAsyncFailFunction()
        {
            // Arrange & Act.
            var result = Retrier.Init()
                                .WithNumberOfRetries(1)
                                .InvokeAsync(FakeService.DivideByZeroExceptionAsync)
                                .WaitForValue();

            // Assert.
            Assert.False(result.Successful);
            Assert.IsType<DivideByZeroException>(result.RetryInfo.Exceptions.FirstOrDefault());
            Assert.Equal(2, result.RetryInfo.Executions);
        }

        [Fact]
        public void RetryAsyncFailFunctionWithSyncFallback()
        {
            // Arrange.
            int expectedResult = 100;

            // Act.
            var result = Retrier.Init()
                                .WithNumberOfRetries(1)
                                .InvokeAsync(FakeService.DivideByZeroExceptionAsync)
                                .WaitForValue()
                                .WithFallBack(() => expectedResult);

            // Assert.
            Assert.Equal(expectedResult, result.Result);
            Assert.False(result.Successful);
            Assert.True(result.ExecutedFallBack);
            Assert.True(result.SuccessfulFallback);
            Assert.IsType<DivideByZeroException>(result.RetryInfo.Exceptions.FirstOrDefault());
            Assert.Equal(2, result.RetryInfo.Executions);
        }

        [Fact]
        public void RetryAsyncFailFunctionWithSyncFallbackValue()
        {
            // Arrange.
            int expectedResult = 100;

            // Act.
            var result = Retrier.Init()
                                .WithNumberOfRetries(1)
                                .InvokeAsync(FakeService.DivideByZeroExceptionAsync)
                                .WaitForValue()
                                .WithFallBackValue(expectedResult);

            // Assert.
            Assert.Equal(expectedResult, result.Result);
            Assert.False(result.Successful);
            Assert.True(result.ExecutedFallBack);
            Assert.True(result.SuccessfulFallback);
            Assert.IsType<DivideByZeroException>(result.RetryInfo.Exceptions.FirstOrDefault());
            Assert.Equal(2, result.RetryInfo.Executions);
        }

        [Fact]
        public async void RetryAsyncFailFunctionWithAsyncFallback()
        {
            // Arrange & Act.
            var result = await Retrier.Init()
                                .WithNumberOfRetries(1)
                                .InvokeAsync(FakeService.DivideByZeroExceptionAsync)
                                .WaitForValue()
                                .WithFallBackAsync(FakeService.ThrowOutOfMemoryAsync);

            // Assert.
            Assert.False(result.Successful);
            Assert.False(result.SuccessfulFallback);
            Assert.True(result.ExecutedFallBack);
            Assert.Equal(3, result.RetryInfo.Executions);
            Assert.IsType<DivideByZeroException>(result.RetryInfo.Exceptions.FirstOrDefault());
            Assert.IsType<OutOfMemoryException>(result.FallBackException);
        }

        [Fact]
        public async void RetryAsyncFailAction()
        {
            // Arrange & Act.
            var result = await Retrier.Init()
                                .WithNumberOfRetries(1)
                                .InvokeAsync(async () => await FakeService.DivideByZeroExceptionAsync());

            // Assert.
            Assert.False(result.Successful);
            Assert.Equal(2, result.RetryInfo.Executions); ;
            Assert.IsType<DivideByZeroException>(result.RetryInfo.Exceptions.FirstOrDefault());
        }

        [Fact]
        public void RetryAsyncFallback()
        {
            // Arrange.
            int zero = 0;

            // Act.
            var result = Retrier.Init()
                                .WithNumberOfRetries(0)
                                .WithMsWaitOf(0)
                                .Invoke(() => 2 / zero)
                                .WithFallBackAsync(async () => await FakeService.GetIntAsync())
                                .WaitForValue();

            // Assert.
            Assert.Equal(1, result.Result);
            Assert.Equal(2, result.RetryInfo.Executions);
            Assert.False(result.Successful);
        }

        [Fact]
        public void RetryAsyncFallbackThatFails()
        {
            // Arrange & Act.
            var result = Retrier.Init()
                                .WithNumberOfRetries(1)
                                .WithMsWaitOf(0)
                                .Invoke(() =>
                                {
                                    throw new OutOfMemoryException();
                                    return 1; // Line to make the compiler happy
                                })
                                .WithFallBackAsync(async () => await FakeService.DivideByZeroExceptionAsync())
                                .WaitForValue();

            // Assert.
            Assert.Equal(0, result.Result);
            Assert.Equal(3, result.RetryInfo.Executions);
            Assert.False(result.Successful);
            Assert.IsType<DivideByZeroException>(result.FallBackException);
            Assert.IsType<OutOfMemoryException>(result.RetryInfo.Exceptions.FirstOrDefault());
        }

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
