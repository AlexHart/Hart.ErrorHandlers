using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using static Hart.Validation.Tests.Constants;

namespace Hart.Validation.Tests
{
    public class ValidationTests
    {
        [Fact]
        public void ValidateEmptyListShouldBeOk()
        {
            // Arrange.
            var rules = new List<ValidationRule>();

            // Act.
            var result = rules.Validate();

            // Assert.
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void ValidateListWithoutErrors()
        {
            // Arrange.
            int id = 200;

            var rules = new List<ValidationRule>
            {
                new ValidationRule(IdMessage, () => id > 1),
                new ValidationRule("Date should be > 2000", 
                    () => DateTime.Now.Year > 2000)
            };

            // Act.
            var result = rules.Validate();

            // Assert.
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void ValidateListWithErrors()
        {
            // Arrange.
            int id = 1;

            var rules = new List<ValidationRule>
            {
                new ValidationRule("always ok", () => true),
                new ValidationRule(IdMessage, () => id > 1),
                new ValidationRule(DateMessage, () => DateTime.Now.Year > 3000)
            };

            // Act.
            var result = rules.Validate();

            // Assert.
            Assert.False(result.IsValid);
            Assert.Equal(2, result.Errors.Count());
            Assert.Contains(IdMessage, result.Errors);
            Assert.Contains(DateMessage, result.Errors);
        }

        [Fact]
        public void ValidateListWithErrorsAdvanced()
        {
            // Arrange.
            var person = new Person(1, "Luke", new DateTime(1951, 9, 25));

            // Act.
            var result = person.Validate();

            // Assert.
            Assert.False(result.IsValid);
            Assert.Equal(2, result.Errors.Count());
            Assert.Contains(IdMessage, result.Errors);
            Assert.Contains(DateMessage, result.Errors);
        }

        [Fact]
        public void ValidateListWithExceptions()
        {
            // Arrange.
            int x = 0;
            var rules = new List<ValidationRule>
            {
                new ValidationRule("always ok", () => true),
                new ValidationRule(InvalidOperation, () => 10 / x == 3),
                new ValidationRule(DateMessage, () => DateTime.Now.Year > 3000)
            };

            // Act.
            var result = rules.Validate();

            // Assert.
            Assert.False(result.IsValid);
            Assert.Equal(2, result.Errors.Count());
            Assert.Contains(InvalidOperation, result.Errors);
            Assert.Contains(DateMessage, result.Errors);
        }
    }
}
