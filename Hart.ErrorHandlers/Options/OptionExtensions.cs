using System;

namespace Hart.ErrorHandlers.Options
{
    public static class OptionExtensions
    {

        public static TOutput Map<TInput, TOutput>(this IOption<TInput> option, Func<IOption<TInput>, TOutput> mapFunction)
        {
            return mapFunction(option);
        }

    }
}