using System;

namespace Hart.ErrorHandlers.Results
{
    /// <summary>
    /// Error with specific exception type.
    /// </summary>
    public class Error<TException> : Error, IResult<TException>
        where TException : Exception
    {
        public new TException Value { get; }

        public new Type GetResultType()
        {
            return typeof(TException);
        }

        public Error(TException exception)
            : base(exception)
        {
            Value = exception;
        }
    }

    /// <summary>
    /// Basic error class with base Exception.
    /// </summary>
    public class Error : IResult
    {
        public Exception Value { get; }

        public bool IsOk => false;

        public bool IsVoid => false;

        public Type GetResultType()
        {
            return typeof(Exception);
        }

        public Error(Exception exception)
        {
            Value = exception;
        }

    }

}
