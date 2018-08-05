using ErrorHandlersTests;
using Hart.ErrorHandlers.Results;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Hart.ErrorHandlers.Tests.Results
{
    public class ErrorHandlerTests
    {

        [Fact]
        public void WrapSuccessAction()
        {
            var result = ErrorHandler.Execute(() =>
            {
                Console.WriteLine("hello world");
            });

            Assert.True(result.IsOk);
            Assert.IsType<Success>(result);
        }

        [Fact]
        public void WrapErrorAction()
        {
            var result = ErrorHandler.Execute(() => throw new Exception(""));

            Assert.False(result.IsOk);
            Assert.IsType<Error>(result);
        }

        [Fact]
        public void WrapSuccessFunc()
        {
            var result = ErrorHandler.Execute(() => 2 + 2);

            Assert.True(result.IsOk);
            Assert.IsType<Success<int>>(result);
            Assert.Equal(4, result.GetSuccess());
        }

        [Fact]
        public void WrapErrorFunc()
        {
            var result = ErrorHandler.Execute(() => FakeService.OutOfMemory());

            Assert.False(result.IsOk);
            Assert.IsType<Error<int>>(result);
        }

    }
}
