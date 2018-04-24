using System;

namespace Hart.ErrorHandlers
{
    public class Error : IResult
    {
        public Exception Exception { get; private set; }

        public Error(Exception exception)
        {
            this.Exception = exception;
        }
    }

}
