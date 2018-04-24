using System;

namespace Hart.ErrorHandlers
{

    /// <summary>
    /// Type that represents a failure in a method call.
    /// </summary>
    public class Error : IResult
    {
        public Exception Exception { get; private set; }

        public Error(Exception exception)
        {
            this.Exception = exception;
        }
    }

}
