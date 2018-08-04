using System;

namespace Hart.ErrorHandlers.Results
{

    public static class ExtensionsResultTuple
    {
        /// <summary>
        /// Indicates if the operation was successful
        /// </summary>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static bool IsSuccessful(this (IResult, Exception) tuple) => tuple.Item1 != null && tuple.Item2 == null;

        /// <summary>
        /// Indicates if the operation was unsuccessful
        /// </summary>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static bool IsUnsuccesful(this (IResult, Exception) tuple) => tuple.IsSuccessful() == false;

        /// <summary>
        /// Generic value getter, will have to be casted.
        /// </summary>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static IResult GetValue(this (IResult, Exception) tuple)
        {
            return tuple.Item1;
        }

        /// <summary>
        /// Generic exception getter, will have to be casted.
        /// </summary>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static Exception GetException(this (IResult, Exception) tuple)
        {
            return tuple.Item2;
        }

        /// <summary>
        /// Generic value getter, will have to be casted.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static IResult<T> GetValue<T>(this (IResult<T>, Exception) tuple)
        {
            return tuple.Item1;
        }

        /// <summary>
        /// Generic exception getter, will have to be casted.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static Exception GetException<T>(this (IResult<T>, Exception) tuple)
        {
            return tuple.Item2;
        }

        /// <summary>
        /// Get Succes value from the result tuple.
        /// </summary>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static Success GetValue(this (Success, Exception) tuple)
        {
            return tuple.Item1;
        }

        /// <summary>
        /// Get typed success value from the result tuple.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static Success<T> GetValue<T>(this (Success<T>, Exception) tuple)
        {
            return tuple.Item1;
        }

        /// <summary>
        /// Get error value from the result tuple.
        /// </summary>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static Error GetValue(this (Error, Exception) tuple)
        {
            return tuple.Item1;
        }

        /// <summary>
        /// Get error value from the result tuple.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static Error<T> GetValue<T>(this (Error<T>, Exception) tuple)
        {
            return tuple.Item1;
        }

    }

}
