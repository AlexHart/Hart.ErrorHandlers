﻿using System;
using System.Collections.Generic;
using System.Text;
using Hart.ErrorHandlers;

namespace Hart.ErrorHandlers.Results
{
    public static class ResultExtensions
    {
        private static bool IsSuccess(this IResult result) => result is Success;

        public static bool IsError(this IResult result) => result is Error;

        #region Unsafe value extractors

        /// <summary>
        /// Get the Success out of an IResult. It will throw an exception if it's not a Success.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static Success GetSuccess(this IResult result)
        {
            if (result.IsSuccess())
                return result as Success;
            
            throw new ArgumentException($"Wrong type {nameof(result)}");
        }

        /// <summary>
        /// Get the Success out of an IResult. It will throw an exception if it's not a Success.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static Success<T> GetSuccess<T>(this IResult result)
        {
            if (result.IsSuccess())
                return result as Success<T>;
            
            throw new ArgumentException($"Wrong type {nameof(result)}");
        }

        /// <summary>
        /// Get the Success out of an IResult. It will throw an exception if it's not a Success.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static Success<T> GetSuccess<T>(this IResult<T> result)
        {
            if (result.IsSuccess())
                return result as Success<T>;

            throw new ArgumentException($"Wrong type {nameof(result)}");
        }

        public static Error GetError(this IResult result)
        {
            if (result.IsError())
                return result as Error;
            
            throw new ArgumentException($"Wrong type {nameof(result)}");
        }

        public static Error<T> GetError<T>(this IResult result)
        {
            if (result.IsError())
                return result as Error<T>;

            throw new ArgumentException($"Wrong type {nameof(result)}");
        }

        #endregion

        #region Safe value extractors

        public static (Success Success, Exception Exception) GetSuccessSafe(this IResult result)
        {
            try
            {
                return (result.GetSuccess(), null);
            }
            catch (Exception ex)
            {
                return (null, ex);
            };
        }

        public static (Success<T> Value, Exception Exception) GetSuccessSafe<T>(this IResult result)
        {
            try
            {
                return (result.GetSuccess<T>(), null);
            }
            catch (Exception ex)
            {
                return (null, ex);
            }
        }

        public static (Error Error, Exception Exception) GetErrorSafe(this IResult result)
        {
            try
            {
                return (result.GetError(), null);
            }
            catch (Exception ex)
            {
                return (null, ex);
            }
        }

        public static (Error<T> Error, Exception Exception) GetErrorSafe<T>(this IResult result)
        {
            try
            {
                return (result.GetError<T>(), null);
            }
            catch (Exception ex)
            {
                return (null, ex);
            }
        }

        #endregion

        #region Tuple helpers

        public static bool IsSuccessful(this (IResult, Exception) tuple) => tuple.Item1 != null && tuple.Item2 == null;

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

        #endregion

    }
}
