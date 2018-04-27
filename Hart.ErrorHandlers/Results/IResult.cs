using System;

namespace Hart.ErrorHandlers.Results
{

    /// <summary>
    /// Type that will be returned instead of throwing exceptions.
    /// You will have to cast it to Success, Error or another type to access the value.
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// Check if it's a successfull request
        /// </summary>
        /// <returns></returns>
        bool IsOk { get; }

        /// <summary>
        /// Check if the method doesn't return a value.
        /// </summary>
        bool IsVoid { get; }

        /// <summary>
        /// Get the result type
        /// </summary>
        /// <returns></returns>
        /// <remarks>If it's a successfull void method will be null</remarks>
        Type GetResultType();

    }

    /// <summary>
    /// Type that will be returned instead of throwing exceptions and returns a value.
    /// You will have to cast it to Success, Error or another type to access the value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IResult<T> : IResult
    {
        T Value { get; }
    }

}
