namespace Hart.ErrorHandlers
{

    /// <summary>
    /// Return type that shows that the function call was successful.
    /// </summary>
    /// <typeparam name="T">Return type</typeparam>
    public class Success<T> : IResult
    {
        public T Value { get; private set; }

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
        public Success()
        {
        }
    }

}
