using System.Collections.Generic;
using static Hart.Validation.Tests.Constants;

namespace Hart.Validation.Tests
{
    public static class PersonExtensions
    {
        public static ValidationResult Validate(this Person person)
        {
            var validationRules = new List<ValidationRule>
            {
                new ValidationRule(
                    "always ok", 
                    () => true),

                new ValidationRule(
                    IdMessage, 
                    () => person.Id > 1),

                new ValidationRule(
                    DateMessage, 
                    () => person.Birthday.Year > 3000),

                new ValidationRule(
                    "Name can't be empty",
                    () => person.Name != string.Empty)
            };

            var result = validationRules.Validate();
            return result;
        }
    }

}
