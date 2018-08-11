using System;

namespace Hart.ErrorHandlers.Results
{

    public static class ExtensionsBindResults
    {

        /// <summary>
        /// Chain two IResults
        /// </summary>
        /// <returns>The bind.</returns>
        /// <param name="resultPrevious">Result previous.</param>
        /// <param name="resultNext">Result next.</param>
        public static IResult Bind(this IResult resultPrevious, Func<IResult> resultNext)
        {
            return resultPrevious.IsOk
                ? resultNext()
                : resultPrevious;
        }

        /// <summary>
        /// Chain two IResults that receive and return values.
        /// </summary>
        /// <returns>The bind.</returns>
        /// <param name="resultPrevious">Result previous.</param>
        /// <param name="resultNext">Result next.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        /// <typeparam name="Y">The 2nd type parameter.</typeparam>
        public static IResult<T> Bind<T, Y>(this IResult<Y> resultPrevious, Func<Y, IResult<T>> resultNext)
        {
            if (resultPrevious.IsOk)
            {
                var value = resultPrevious.GetSuccess<Y>().Value;
                return resultNext(value);
            }

            return resultPrevious.GetError<T>();
        }

        /// <summary>
        /// Chain two IResults that receive and return values.
        /// </summary>
        /// <returns>The bind.</returns>
        /// <param name="resultPrevious">Result previous.</param>
        /// <param name="resultNext">Result next.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static IResult Bind<T>(this IResult<T> resultPrevious, Func<T, IResult> resultNext)
        {
            if (resultPrevious.IsOk)
            {
                var value = resultPrevious.GetSuccess().Value;
                return resultNext(value);
            }

            return resultPrevious;
        }

    }

}
