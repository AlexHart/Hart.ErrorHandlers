using System;
using System.Collections;
using System.Security;

namespace Hart.Validation
{
    public class ValidationRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Hart.Validation.ValidationRule"/> struct.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        /// <param name="validationFunction">Validation function that should be true</param>
        /// <remarks>The validation function is a closure so be carefull what you pass to it if it has to be disposed fast</remarks>
        public ValidationRule(string errorMessage, Func<bool> validationFunction)
        {
            ErrorMessage = errorMessage;

            try
            {
                IsValid = validationFunction();
            }
            catch (Exception)
            {
                IsValid = false;
            }
        }

        public string ErrorMessage { get; }

        public bool IsValid { get; }
    }
}
