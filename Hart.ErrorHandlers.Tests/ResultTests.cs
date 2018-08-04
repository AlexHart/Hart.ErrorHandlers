using Hart.ErrorHandlers;
using System;
using Xunit;
using System.Linq;
using Hart.ErrorHandlers.Results;

namespace ErrorHandlersTests.Results
{

    public class ResultTests
    {
        [Fact]
        public void DivideByZeroHandled()
        {
            // Arrange.
            // Act.
            IResult result = FakeService.DoDivision(10, 0);

            // Assert.
            Assert.True(result is Error);
            Assert.True(result.IsError());
        }

        [Fact]
        public void DivideByZeroHandledWithSpecificException()
        {
            // Arrange.
            // Act.
            IResult<double> result = FakeService.DoDivision(10, 0);

            // Assert.
            Assert.True(result is Error);
            Assert.True(result is Error<double>);
            Assert.True(result.IsError());
            Assert.Equal(typeof(DivideByZeroException), result.GetResultType());
        }

        [Fact]
        public void DivideWithoutErrorHandled()
        {
            // Arrange.
            // Act.
            IResult result = FakeService.DoDivision(10, 2);

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
            // Act.
            IResult result = FakeService.BooleanOperation(() => 1 == 1);

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
            // Act.
            IResult result = FakeService.FakeVoidMethod("Demo fake method");

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
            // Act.
            IResult result = FakeService.FakeVoidMethod(string.Empty);

            // Assert.
            Assert.False(result is Success);
            Assert.True(result is Error);
            Assert.False(result.IsVoid); // Is not void because it returns an exception.
        }

        [Fact]
        public void VoidMethodTypeIsNull()
        {
            // Arrange.
            // Act.
            IResult result = FakeService.FakeVoidMethod("test");

            // Assert.
            Assert.Null((result as Success).GetResultType());
        }

        [Fact]
        public void ExtractSuccess()
        {
            // Arrange.
            // Act.
            IResult result = FakeService.DoDivision(10, 2);
            Success successUntyped = result.GetSuccess();

            // Assert.
            Assert.True(successUntyped is Success);
            Assert.True(successUntyped is Success<double>);
        }

        [Fact]
        public void ExtractSuccessTyped()
        {
            // Arrange.
            // Act.
            IResult result = FakeService.DoDivision(10, 2);
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
            // Act.
            IResult<double> result = FakeService.DoDivision(10, 0);
            Error errorUntyped = result.GetError();
            Type exceptionType = errorUntyped.GetResultType();

            // Assert.
            Assert.True(errorUntyped is Error);
            Assert.True(errorUntyped is Error<double>);
            Assert.Equal(typeof(DivideByZeroException), exceptionType);
        }

        [Fact]
        public void ExtractErrorTyped()
        {
            // Arrange.
            // Act.
            IResult result = FakeService.DoDivision(10, 0);
            var errorTyped = result.GetError<double>();
            Type exceptionType = errorTyped.GetResultType();

            // Assert.
            Assert.True(errorTyped is Error);
            Assert.Equal(typeof(DivideByZeroException), exceptionType);
            Assert.False(errorTyped.IsOk);
            Assert.False(errorTyped.IsVoid);
        }

        [Fact]
        public void ExtractSuccessSafely()
        {
            // Arrange.
            // Act.
            IResult result = FakeService.DoDivision(10, 2);
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
            // Act.
            IResult result = FakeService.DoDivision(10, 2);
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
            // Act.
            IResult result = FakeService.DoDivision(10, 0);
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
            // Act.
            var result = FakeService.DoDivision(10, 0);
            (Error<double>, Exception) errorTuple = result.GetErrorSafe<double>();

            var error = errorTuple.GetValue();

            // Assert.
            Assert.True(error is Error<double>);
            Assert.Equal(typeof(DivideByZeroException), error.GetResultType());
            Assert.Equal(typeof(double), error.Value.GetType());
        }

        [Fact]
        public void DontAllowExtractingValueFromResultOnlySuccess()
        {
            // Arrange.
            IResult<double> result = FakeService.DoDivision(10, 2);

            // Act.
            var success = result.GetSuccess<double>();
            var value = success.Value;

            // Assert.
            Assert.Equal(5.0, value);
        }

        [Fact]
        public void DontAllowExtractingValueSafelyFromResultOnlySuccess()
        {
            // Arrange.
            IResult<double> operationResult = FakeService.DoDivision(10, 2);

            // Act.
            Success<double> success = operationResult.GetSuccessSafe<double>().Value;

            // Assert.
            Assert.Equal(5.0, success.Value);
        }

        [Fact]
        public void BindMultipleResultsTogetherOk()
        {
            var res = FakeService.DoDivision(10, 2)
                .Bind(() => FakeService.DoDivision(20, 2))
                .Bind(() => FakeService.DoDivision(4, 2));

            Assert.True(res.IsOk);
        }

        [Fact]
        public void BindMultipleResultsTogetherError()
        {
            var res = FakeService.DoDivision(10, 2)
                .Bind(() => FakeService.DoDivision(20, 0))
                .Bind(() => FakeService.DoDivision(4, 2));

            Assert.False(res.IsOk);
        }

        [Fact]
        public void BindMultipleResultsWithValueTogetherOk()
        {
            var res = FakeService.DoDivision(200, 2)
                .Bind((x) => FakeService.DoDivision(x, 2))
                .Bind((x) => FakeService.DoDivision(x, 2));

            Assert.True(res.IsOk);
            Assert.Equal(25.0, res.GetSuccess<double>().Value);
        }

        [Fact]
        public void BindMultipleResultsWithValueTogetherError()
        {
            var res = FakeService.DoDivision(100, 2)
                .Bind((x) => FakeService.DoDivision((int)x, 0))
                .Bind((x) => FakeService.DoDivision((int)x, 2));

            Assert.False(res.IsOk);
            Assert.IsType<DivideByZeroException>(res.GetError<double>().ExceptionValue);
        }

        [Fact]
        public void BindDifferentTypesTogether()
        {
            var res = FakeService.DoDivision(100, 2)
                .Bind((x) => FakeService.DoStringStuff(x.ToString()));

            Assert.True(res.IsOk);
            Assert.Equal("*** 50 ***", res.GetSuccess<string>().Value);
        }

        [Fact]
        public void BindDifferentTypesTogetherIgnoringPreviousResult()
        {
            //This should never be done, but for the shake of it...
            var res = FakeService.DoDivision(100, 2)
                .Bind((x) => FakeService.DoStringStuff(x.ToString()))
                .Bind((s) => FakeService.DoDivision(10, 2));

            Assert.True(res.IsOk);
            Assert.Equal(5.0, res.GetSuccess<double>().Value);
        }

        [Fact]
        public void BindDifferentTypesTogetherWithOnesThatOnlyReturnResult()
        {
            IResult res = FakeService.DoDivision(100, 2)
                .Bind((x) => FakeService.DoStringStuff(x.ToString()))
                .Bind((x) => FakeService.FakeVoidMethod(x));

            Assert.True(res.IsOk);
        }
    }
}
