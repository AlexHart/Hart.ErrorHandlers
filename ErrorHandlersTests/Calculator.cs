using System;
using System.Collections.Generic;
using System.Text;
using Hart.ErrorHandlers;
using Hart.ErrorHandlers.Results;

namespace ErrorHandlersTests
{

    /// <summary>
    /// Class for testing.
    /// </summary>
    public class Calculator
    {

        public IResult DoDivision(int x, int y)
        {
            IResult result;
            try
            {
                result = new Success<double>(x / y);
            }
            catch(DivideByZeroException ex)
            {
                result = new Error<DivideByZeroException>(ex);
            }
            catch (Exception ex)
            {
                result = new Error(ex);
            }
            return result;
        }

        public IResult BooleanOperation(Func<bool> fun)
        {
            IResult result;
            try
            {
                var boolRes = fun.Invoke();
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
        public IResult FakeVoidMethod(string message)
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

    }
}
