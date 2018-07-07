using System;

namespace Hart.ErrorHandlers.Results
{
    /// <summary>
    /// Error with specific exception type.
    /// </summary>
    public class Error<TType> : Error, IResult<TType>
    {
        /// <summary>
        /// Default value of the type asociated with this error.
        /// </summary>
        public TType Value => default(TType);

        /// <summary>
        /// Constructor for an error associated with a type.
        /// </summary>
        /// <param name="exception"></param>
        public Error(Exception exception) : base(exception)
        {
        }

    }

    /// <summary>
    /// Basic error class with base Exception.
    /// </summary>
    public class Error : IResult
    {
        public Exception ExceptionValue { get; }

        public bool IsOk => false;

        public bool IsVoid => false;

        /// <summary>
        /// Get the result type of the exception.
        /// </summary>
        /// <returns></returns>
        public Type GetResultType()
        {
            return ExceptionValue.GetType();
        }

        /// <summary>
        /// Constructor for an error.
        /// </summary>
        /// <param name="exception"></param>
        public Error(Exception exception)
        {
            this.ExceptionValue = exception;
        }
    }

}
