using System.Collections.Generic;
using System.Linq;

namespace Hart.Validation
{
    public static class ValidationExtensions
    {
        public static ValidationResult Validate(this IEnumerable<ValidationRule> rules)
        {
            var errors = rules
                .Where(x => !x.IsValid)
                .Select(x => x.ErrorMessage)
                .ToList();

            return new ValidationResult(!errors.Any(), errors);
        }
    }
}
