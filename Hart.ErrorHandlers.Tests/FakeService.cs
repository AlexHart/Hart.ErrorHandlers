using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hart.ErrorHandlers;
using Hart.ErrorHandlers.Results;

namespace ErrorHandlersTests
{

    /// <summary>
    /// Class for testing.
    /// </summary>
    public class FakeService
    {

        public static IResult<double> DoDivision(int x, int y)
        {
            IResult<double> result;
            try
            {
                result = new Success<double>(x / y);
            }
            catch(DivideByZeroException ex)
            {
                result = new Error<double>(ex);
            }
            catch (Exception ex)
            {
                result = new Error<double>(ex);
            }
            return result;
        }

        public static IResult<double> DoDivision(double x, double y)
        {
            IResult<double> result;
            try
            {
                result = new Success<double>(x / y);
            }
            catch(DivideByZeroException ex)
            {
                result = new Error<double>(ex);
            }
            catch (Exception ex)
            {
                result = new Error<double>(ex);
            }
            return result;
        }
        

        public static IResult BooleanOperation(Func<bool> fun)
        {
            IResult result;
            try
            {
                var boolRes = fun();
                result = new Success<bool>(boolRes);
	        }
            catch (Exception ex)
            {
                result = new Error(ex);                
            }
            return result;
        }

        /// <summary>
        /// Method that returns IResult with success or error, substitutes a void in C# or sub call in VB.net
        /// </summary>
        /// <returns></returns>
        public static IResult FakeVoidMethod(string message)
        {
            IResult result;

            // Let's preted that string.empty makes this method fail.
            if (string.IsNullOrEmpty(message))
            {
                // Return an error with the exception inside.
                result = new Error(new ArgumentException(nameof(message)));
            }
            else
            {
                // Return that everything was successful.
                result = new Success();
            }

            return result;
        }

        /// <summary>
        /// Just concatenate som garbage to a string.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IResult<string> DoStringStuff(string message)
        {
            return new Success<string>("*** " + message + " ***");
        }

        /// <summary>
        /// Method to fake an async call.
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetHelloWorldAsync()
        {
            return await Task.Run(() => "Hello world");
        }

        /// <summary>
        /// Method to fake an async call.
        /// </summary>
        /// <returns></returns>
        public static async Task<int> GetIntAsync()
        {
            return await Task.Run(() => 1);
        }

        public static async Task<int> DivideByZeroExceptionAsync()
        {
            return await Task.Run(() =>
            {
                int zero = 0;
                return 2 / zero;
            });
        }

        public static async Task<int> ThrowOutOfMemoryAsync()
        {
            return await Task.Run(() =>
            {
                throw new OutOfMemoryException();
                return 0;
            });
        }

        /// <summary>
        /// Method to fake an async call.
        /// </summary>
        /// <returns></returns>
        public static async Task DoFakeCalculationsAsync()
        {
            await Task.Run(() => Thread.Sleep(10));
        }
    }
}
