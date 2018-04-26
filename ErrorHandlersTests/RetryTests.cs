using Hart.ErrorHandlers;
using System;
using Xunit;
using System.Linq;
using Hart.ErrorHandlers.Retry;

namespace ErrorHandlersTests
{

    public class RetrierTests
    {

        [Fact]
        public void RetryFunctionOk()
        {
            // Arrange
            int res = 0;
            int res2 = 0;
            using (var retrier = new Retrier<int>())
            {
                // Act.
                res = retrier
                    .WithMsWaitOf(200)
                    .WithNumberOfRetries(3)
                    .Invoke(() => 2 + 2);

                res2 = retrier.Invoke(() => 5 * 2);
            }

            // Assert.
            Assert.Equal(4, res);
            Assert.Equal(10, res2);
        }

        [Fact]
        public void RetryFunctionOkWithoutUsing()
        {
            // Arrange
            int res = 0;
            int res2 = 0;
            var retrier = new Retrier<int>();
            
            // Act.
            res = retrier
                .WithMsWaitOf(1000)
                .WithNumberOfRetries(4)
                .Invoke(() => 2 + 2);

            res2 = retrier.Invoke(() => 5 * 2);
            
            // Assert.
            Assert.Equal(4, res);
            Assert.Equal(10, res2);
            Assert.NotNull(retrier);
            Assert.NotNull(retrier.Config);
            Assert.Equal(1000, retrier.Config.MsWait);
            Assert.Equal(4, retrier.Config.MaxRetries);
        }

        [Fact]
        public void RetryMultipleMethodsUpdatingConfig()
        {
            // Arrange
            int res = 0;
            int res2 = 0;
            var retrier = new Retrier<int>();

            // Act.
            res = retrier
                .WithMsWaitOf(1000)
                .WithNumberOfRetries(2)
                .Invoke(() => 2 + 2);

            Assert.Equal(1000, retrier.Config.MsWait);
            Assert.Equal(2, retrier.Config.MaxRetries);

            res2 = retrier
                    .WithMsWaitOf(500)
                    .WithNumberOfRetries(4)
                    .Invoke(() => 5 * 2);

            Assert.Equal(500, retrier.Config.MsWait);
            Assert.Equal(4, retrier.Config.MaxRetries);
        }

        [Fact]
        public void RetryFunctionWrong()
        {
            // Arrange & Act.
            var ex = Record.Exception(() =>
            {
                var retrier = new Retrier<int>();
                int zero = 0;
                int res = retrier
                            .WithMsWaitOf(0)
                            .WithNumberOfRetries(3)
                            .Invoke(() => 2 / zero);
            });

            // Assert.
            Assert.NotNull(ex);
            Assert.IsType<DivideByZeroException>(ex);
        }

    }
}
