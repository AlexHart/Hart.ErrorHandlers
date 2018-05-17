using System;
using Xunit;
using System.Linq;
using Hart.ErrorHandlers.Retry;
using System.Diagnostics;

namespace ErrorHandlersTests.Retry
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

    }
}
