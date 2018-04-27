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
            Assert.True(res.RetryInfo.Successful);
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
            // Arrange
            Func<int> onFail = () => 2 + 2;

			// Act.
            var config = Retrier.Init(200, 10);

            // Assert.
            Assert.Equal(200, config.MsWait);
            Assert.Equal(10, config.MaxRetries);
        }

        //[Fact]
        //public void RetryFunctionOk()
        //{
        //    // Arrange
        //    int res = 0;

        //    // Act.
        //    res = Retrier.Init()
        //                 .WithMsWaitOf(200)
        //                 .WithNumberOfRetries(3)
        //                 .InvokeSafe(() => 2 + 2)
        //                 .Result
        //                 .GetSuccess<int>()
        //                 .Value;

        //    // Assert.
        //    Assert.Equal(4, res);
        //}

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

        //[Fact]
        //public void RetryFunctionWithExceptionAndFailFallback()
        //{
        //    // Arrange & Act.
        //    var ex = Record.Exception(() =>
        //    {
        //        int zero = 0;
        //        int res = Retrier.Init()
        //                         .WithMsWaitOf(0)
        //                         .WithNumberOfRetries(1)
        //                         .OnFail<int>(() => throw new ArgumentException("test"))
        //                         .Invoke(() => 2 / zero);
        //    });

        //    // Assert.
        //    Assert.NotNull(ex);

        //    //TODO: Should throw the Invoke exception or the OnFail exception?
        //    Assert.IsType<ArgumentException>(ex);
        //}

        //[Fact]
        //public void RetryFunctionWithExceptionAndFailFallbackSafe()
        //{
        //    // Arrange.
        //    int res = 0;
        //    bool successful = true;

        //    // Act.
        //    var ex = Record.Exception(() =>
        //    {
        //        int zero = 0;
        //        RetryResult<IResult> funcRes = Retrier.Init<int>()
        //                             .WithMsWaitOf(0)
        //                             .WithNumberOfRetries(1)
        //                             .OnFail<int>(() => 123)
        //                             .InvokeSafe(() => 2 / zero);

        //        // It wasn't successful because it went to the onfail function.
        //        successful = funcRes.Successful;

        //        res = funcRes.Result.GetSuccess<int>().Value;
        //    });

        //    // Assert.
        //    Assert.Null(ex);
        //    Assert.Equal(123, res);
        //    Assert.False(successful);
        //}

        //[Fact]
        //public void RetryFunctionWithFallback()
        //{
        //    // Arrange & Act.
        //    int zero = 0;

        //    int res = Retrier.Init()
        //                     .WithMsWaitOf(0)
        //                     .WithNumberOfRetries(1)
        //                     .OnFail<int>(() => 123)
        //                     .Invoke(() => 2 / zero);

        //    // Assert.
        //    Assert.Equal(123, res);
        //}

        //[Fact]
        //public void RetryFunctionWithFallbackReturnsSuccess()
        //{
        //    // Arrange.
        //    int zero = 0;

        //    // Act.
        //    var res = Retrier.Init()
        //                     .WithMsWaitOf(0)
        //                     .WithNumberOfRetries(1)
        //                     .OnFail<IResult>(() => new Success())
        //                     .Invoke(() => {
        //                         int val = 2 / zero;
        //                         return new Success<int>(val);
        //                     });

        //    // Assert.
        //    Assert.True(res is Success);
        //}

        //[Fact]
        //public void RetryFunctionWithInvokeSafe()
        //{
        //    // Arrange.
        //    int zero = 0;

        //    // Act.
        //    var res = Retrier.Init()
        //                     .WithMsWaitOf(0)
        //                     .WithNumberOfRetries(1)
        //                     .OnFail<int>(() => throw new ArgumentNullException())
        //                     .InvokeSafe(() => 2 / zero)
        //                     .Result;

        //    // Assert.
        //    Assert.True(res is Error);
        //    Assert.Equal(typeof(ArgumentNullException), (res as Error).Value.GetType());
        //}

    }
}
