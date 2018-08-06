namespace Hart.ErrorHandlers.Options
{
    public struct Some<T> : IOption<T>
    {
        /// <summary>
        /// Value wrapper in the some holder.
        /// </summary>
        private T Value { get; }

        /// <summary>
        /// Constructor to set the value.
        /// </summary>
        /// <param name="value"></param>
        public Some(T value) => Value = value;

        /// <summary>
        /// Implicit operator to get the value from the Some.
        /// </summary>
        /// <param name="some"></param>
        public static implicit operator T(Some<T> some)
        {
            return some.Value;
        }
    }

}
