using System;
using System.Collections.Generic;
using System.Text;

namespace Hart.ErrorHandlers.Options
{

    public interface IOption<T> { }

    public static class Option {

        public static Some<T> Some<T>(T value) => new Some<T>(value);

        public static None<T> None<T>() => new None<T>();

    }

    public sealed class Some<T> : IOption<T>
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

    public sealed class None<T> : IOption<T>
    {
        public static None<T> Value { get => Option.None<T>(); }
    }

}
