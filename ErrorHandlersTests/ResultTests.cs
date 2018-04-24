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
        public void DivideByZeroHandledWithSpecificException()
        {
            // Arrange.
            var calculator = new Calculator();

            // Act.
            IResult result = calculator.DoDivision(10, 0);

            // Assert.
            Assert.True(result is Error);
            Assert.True(result is Error<DivideByZeroException>);
            Assert.Equal(typeof(DivideByZeroException), result.GetResultType());
        }

        [Fact]
        public void DivideWithoutErrorHandled()
        {
            // Arrange.
            var calculator = new Calculator();

            // Act.
            IResult result = calculator.DoDivision(10, 2);

            // Assert.
            Assert.True(result is Success);
            Assert.Equal(5, (result as Success<double>).Value);
            Assert.Equal(typeof(double), result.GetResultType());
        }

        /// <summary>
        /// Test to ensure that works with boolean values too.
        /// </summary>
        [Fact]
        public void ReturnBoolAsGenericTest()
        {
            // Arrange.
            var calculator = new Calculator();

            // Act.
            IResult result = calculator.BooleanOperation(() => 1 == 1);

            // Assert.
            Assert.True(result is Success);
            Assert.True(result is Success<bool>);
            Assert.Equal(typeof(bool), result.GetResultType());
            Assert.True((result as Success<bool>).Value);
            Assert.False(result.IsVoid);
        }


        /// <summary>
        /// Call to a method that doesn't fail.
        /// </summary>
        [Fact]
        public void CallToOkVoidMethod()
        {
            // Arrange.
            var calculator = new Calculator();

            // Act.
            IResult result = calculator.FakeVoidMethod("Demo fake method");

            // Assert.
            Assert.True(result is Success);
            Assert.False(result is Error);
            Assert.True(result.IsVoid);
        }

        /// <summary>
        /// Call to a method that doesn't fail.
        /// </summary>
        [Fact]
        public void CallToWrongVoidMethod()
        {
            // Arrange.
            var calculator = new Calculator();

            // Act.
            IResult result = calculator.FakeVoidMethod(string.Empty);

            // Assert.
            Assert.False(result is Success);
            Assert.True(result is Error);
            Assert.False(result.IsVoid); // Is not void because it returns an exception.
        }

        [Fact]
        public void VoidMethodTypeIsNull()
        {
            // Arrange.
            var calculator = new Calculator();

            // Act.
            IResult result = calculator.FakeVoidMethod("test");

            // Assert.
            Assert.Null((result as Success).GetResultType());
        }
    }
}
