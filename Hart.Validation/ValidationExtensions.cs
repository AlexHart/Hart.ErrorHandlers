using System.Collections.Generic;
using System.Linq;

namespace Hart.Validation
{
    public static class ValidationExtensions
    {
        public static ValidationResult Validate(this IEnumerable<ValidationRule> rules)
        {
            var errors = new List<string>();

            foreach (var rule in rules)
            {
                if (!rule.Validate())
                {
                    errors.Add(rule.ErrorMessage);
                }
            }

            return new ValidationResult(!errors.Any(), errors);
        }
    }
}
