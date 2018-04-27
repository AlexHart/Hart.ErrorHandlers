using System;

namespace Hart.ErrorHandlers.Results
{

    /// <summary>
    /// Error with specific exception type.
    /// </summary>
    public class Error<TException> : Error, IResult<TException>
            where TException : Exception
    {
        public new TException Value { get; private set; }

        public new Type GetResultType() => typeof(TException);

        public Error(TException exception)
            : base(exception)
        {
            this.Value = exception;
        }

    }

    /// <summary>
    /// Basic error class with base Exception.
    /// </summary>
    public class Error : IResult
    {
        public Exception Value { get; protected set; }

        public bool IsOk => false;

        public bool IsVoid => false;

        public Type GetResultType() => typeof(Exception);

        public Error(Exception exception)
        {
            this.Value = exception;
        }

    }

}
