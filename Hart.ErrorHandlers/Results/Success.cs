using System;

namespace Hart.ErrorHandlers.Results
{

    /// <summary>
    /// Return type that shows that the function call was successful.
    /// </summary>
    /// <typeparam name="T">Return type</typeparam>
    public class Success<T> : Success, IResult<T>
    {
        public T Value { get; private set; }

        public new Type GetResultType() => typeof(T);

        public new bool IsVoid => false;

        public Success(T value)
        {
            this.Value = value;
        }
    }

    /// <summary>
    /// Return type that shows that the method call was successful.
    /// </summary>
    public class Success : IResult
    {
        public bool IsOk => true;

        public bool IsVoid => true;

        public Type GetResultType() => null;
    }

}
