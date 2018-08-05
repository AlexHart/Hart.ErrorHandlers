using System;
using System.Collections.Generic;
using System.Text;

namespace Hart.ErrorHandlers.Options
{

    public interface IOption { }

    public sealed class Some<T> : IOption
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

    public sealed class None : IOption
    {
        public static None Value { get => new None(); }
    }

}
