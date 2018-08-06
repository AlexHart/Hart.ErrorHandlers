namespace Hart.ErrorHandlers.Options
{
    public struct None<T> : IOption<T>
    {
        public static None<T> Value { get => Option.None<T>(); }
    }

}
