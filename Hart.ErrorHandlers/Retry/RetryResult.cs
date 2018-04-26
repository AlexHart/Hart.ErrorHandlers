using System;
namespace Hart.ErrorHandlers
{
    public class RetryResult<T>
    {

        public T Result { get; set; }

        public bool Successful { get; set; }

        //TODO: Implement a counter for the number of tries.
    }
}
