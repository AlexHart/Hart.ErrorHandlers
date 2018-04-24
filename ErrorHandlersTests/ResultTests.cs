using Hart.ErrorHandlers;
using System;
using Xunit;

namespace ErrorHandlersTests
{

    public class ResultTests
    {
        [Fact]
        public void DivideByZeroHandled()
        {
            // Arrange.
            var calculator = new Calculator();

            // Act.
            IResult result = calculator.DoDivision(10, 0);

            // Assert.
            Assert.True(result is Error);
        }

        [Fact]
        public void DivideWithoutErrorHandled()
        {
            // Arrange.
            var calculator = new Calculator();

            // Act.
            IResult result = calculator.DoDivision(10, 2);

            // Assert.
            Assert.True(result is Success<double>);
            Assert.Equal(5, (result as Success<double>).Value);
        }
    }
}
