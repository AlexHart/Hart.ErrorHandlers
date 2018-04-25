using Hart.ErrorHandlers;
using System;
using Xunit;
using System.Linq;

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
            Assert.True(result.IsError());
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
            Assert.True(result.IsError());
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

        [Fact]
        public void ExtractSuccess()
        {
            // Arrange.
            var calculator = new Calculator();

            // Act.
            IResult result = calculator.DoDivision(10, 2);
            Success successUntyped = result.GetSuccess();

            // Assert.
            Assert.True(successUntyped is Success);
            Assert.True(successUntyped is Success<double>);
        }

        [Fact]
        public void ExtractSuccessTyped()
        {
            // Arrange.
            var calculator = new Calculator();

            // Act.
            IResult result = calculator.DoDivision(10, 2);
            Success<double> successTyped = result.GetSuccess<double>();

            // Assert.
            Assert.True(successTyped is Success);
            Assert.True(successTyped.IsOk);
            Assert.False(successTyped.IsVoid);
        }

        [Fact]
        public void ExtractError()
        {
            // Arrange.
            var calculator = new Calculator();

            // Act.
            IResult result = calculator.DoDivision(10, 0);
            Error errorUntyped = result.GetError();

            // Assert.
            Assert.True(errorUntyped is Error);
            Assert.True(errorUntyped is Error<DivideByZeroException>);
        }

        [Fact]
        public void ExtractErrorTyped()
        {
            // Arrange.
            var calculator = new Calculator();

            // Act.
            IResult result = calculator.DoDivision(10, 0);
            var errorTyped = result.GetError<DivideByZeroException>();

            // Assert.
            Assert.True(errorTyped is Error);
            Assert.True(errorTyped is Error<DivideByZeroException>);
            Assert.False(errorTyped.IsOk);
            Assert.False(errorTyped.IsVoid);
        }

        [Fact]
        public void ExtractSuccessSafely()
        {
            // Arrange.
            var calculator = new Calculator();

            // Act.
            IResult result = calculator.DoDivision(10, 2);
            (Success, Exception) tuple = result.GetSuccessSafe();

            Success success = tuple.GetValue();

            // Assert.
            Assert.True(success is Success);
            Assert.NotNull(success);
            Assert.True(tuple.IsSuccessful());
            Assert.NotNull(tuple.Item1);
            Assert.Null(tuple.Item2);
        }

        [Fact]
        public void ExtractSuccessTypedSafely()
        {
            // Arrange.
            var calculator = new Calculator();

            // Act.
            IResult result = calculator.DoDivision(10, 2);
            (Success<double>, Exception) tuple = result.GetSuccessSafe<double>();

            var success = tuple.GetValue();

            // Assert.
            Assert.True(success is Success);
            Assert.True(success is Success<double>);
            Assert.Equal(5, success.Value);
            Assert.True(tuple.IsSuccessful());
            Assert.NotNull(tuple.Item1);
            Assert.Null(tuple.Item2);
        }

        [Fact]
        public void ExtractErrorSafely()
        {
            // Arrange.
            var calculator = new Calculator();

            // Act.
            IResult result = calculator.DoDivision(10, 0);
            (Error, Exception) tuple = result.GetErrorSafe();

            Error error = tuple.GetValue();

            // Assert.
            Assert.True(error is Error);
            Assert.True(tuple.IsSuccessful());
            Assert.False(tuple.IsUnsuccesful());
            Assert.NotNull(tuple.Item1);
            Assert.Null(tuple.Item2);
        }

        [Fact]
        public void ExtractErrorTypedSafely()
        {
            // Arrange.
            var calculator = new Calculator();

            // Act.
            IResult result = calculator.DoDivision(10, 0);
            (Error<DivideByZeroException>, Exception) errorTuple = result.GetErrorSafe<DivideByZeroException>();

            Error<DivideByZeroException> error = errorTuple.GetValue();

            // Assert.
            Assert.True(error is Error<DivideByZeroException>);
            Assert.Equal(typeof(DivideByZeroException), error.Value.GetType());
        }
    }
}
