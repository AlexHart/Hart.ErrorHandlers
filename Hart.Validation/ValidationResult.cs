using System.Collections.Generic;

namespace Hart.Validation
{
    public class ValidationResult
    {
        public ValidationResult(bool isValid, IEnumerable<string> errors)
        {
            IsValid = isValid;
            Errors = errors;
        }

        public bool IsValid { get; }
        public IEnumerable<string> Errors { get; }

    }
}
