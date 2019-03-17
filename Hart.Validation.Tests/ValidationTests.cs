using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;

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
                new ValidationRule("id should be bigger than 1", () => id > 1),
                new ValidationRule("Date should be > 2000", () => DateTime.Now.Year > 2000)
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

            string idMessage = "id should be bigger than 1";
            string dateMessage = "Date should be > 3000";

            var rules = new List<ValidationRule>
            {
                new ValidationRule("always ok", () => true),
                new ValidationRule(idMessage, () => id > 1),
                new ValidationRule(dateMessage, () => DateTime.Now.Year > 3000)
            };

            // Act.
            var result = rules.Validate();

            // Assert.
            Assert.False(result.IsValid);
            Assert.Equal(2, result.Errors.Count());
            Assert.Contains(idMessage, result.Errors);
            Assert.Contains(dateMessage, result.Errors);
        }
    }
}
