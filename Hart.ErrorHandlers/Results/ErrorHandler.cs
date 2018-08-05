using System;
using System.Collections.Generic;
using System.Text;

namespace Hart.ErrorHandlers.Results
{
    public static class ErrorHandler
    {
        /// <summary>
        /// Execute func safely and wrap the return inside an IResult of T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fun"></param>
        /// <returns></returns>
        public static IResult<T> Execute<T>(Func<T> fun)
        {
            try
            {
                var funResult = fun();
                return new Success<T>(funResult);
            }
            catch (Exception ex)
            {
                return new Error<T>(ex);
            }
        }

        /// <summary>
        /// Execute Action safely and wrap up the return inside an IResult.
        /// </summary>
        /// <param name="fun"></param>
        /// <returns></returns>
        public static IResult Execute(Action fun)
        {
            try
            {
                fun();
                return new Success();
            }
            catch (Exception ex)
            {
                return new Error(ex);
            }
        }

    }
}
