using System;
using Xunit;
using System.Linq;
using Hart.ErrorHandlers.Retry;
using System.Diagnostics;
using ErrorHandlersTests.Helpers;

namespace ErrorHandlersTests.Retry
{

    public class RetrierFallbackTests
    {

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
                                    return FakeService.OutOfMemory();
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

    }
}
