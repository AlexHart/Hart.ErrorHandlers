using System;
using System.Collections.Generic;
using System.Text;
using Hart.ErrorHandlers;

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
            catch (Exception ex)
            {
                result = new Error(ex);
            }
            return result;
        }

    }
}
