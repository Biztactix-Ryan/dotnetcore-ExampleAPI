using ExampleAPI.Contracts.V1;
using ExampleAPI.Contracts.V1.Validators;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace ExampleAPI.Tests.Validators
{
    public class ExampleObjectCreateValidatorTests
    {
        private readonly ExampleObjectCreateValidator _validator = new();

        [Fact]
        public void Valid_Object_ShouldPassValidation()
        {
            var model = new ExampleObjectCreate
            {
                FirstName = "John",
                LastName = "Doe",
                Phone = "1234567890"
            };

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FirstName_WhenEmpty_ShouldFail(string firstName)
        {
            var model = new ExampleObjectCreate { FirstName = firstName, LastName = "Doe" };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.FirstName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void LastName_WhenEmpty_ShouldFail(string lastName)
        {
            var model = new ExampleObjectCreate { FirstName = "John", LastName = lastName };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.LastName);
        }

        [Theory]
        [InlineData("John123")]
        [InlineData("John Doe")]
        [InlineData("John-Doe")]
        [InlineData("John_Doe")]
        public void FirstName_WithNonAlphaChars_ShouldFail(string firstName)
        {
            var model = new ExampleObjectCreate { FirstName = firstName, LastName = "Doe" };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.FirstName);
        }

        [Theory]
        [InlineData("Smith123")]
        [InlineData("O'Brien")]
        [InlineData("Smith-Jones")]
        public void LastName_WithNonAlphaChars_ShouldFail(string lastName)
        {
            var model = new ExampleObjectCreate { FirstName = "John", LastName = lastName };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.LastName);
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("123-456")]
        [InlineData("+1234567890")]
        public void Phone_WithNonNumericChars_ShouldFail(string phone)
        {
            var model = new ExampleObjectCreate
            {
                FirstName = "John",
                LastName = "Doe",
                Phone = phone
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Phone);
        }

        [Fact]
        public void Phone_WhenNull_ShouldPass()
        {
            var model = new ExampleObjectCreate
            {
                FirstName = "John",
                LastName = "Doe",
                Phone = null
            };

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.Phone);
        }

        [Fact]
        public void Phone_WhenEmpty_ShouldFail()
        {
            var model = new ExampleObjectCreate
            {
                FirstName = "John",
                LastName = "Doe",
                Phone = ""
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Phone);
        }
    }

    public class ExampleObjectUpdateValidatorTests
    {
        private readonly ExampleObjectUpdateValidator _validator = new();

        [Fact]
        public void Valid_Object_ShouldPassValidation()
        {
            var model = new ExampleObjectUpdate
            {
                FirstName = "John",
                LastName = "Doe",
                Phone = "1234567890"
            };

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void AllNull_ShouldPass_BecauseUpdateFieldsAreOptional()
        {
            var model = new ExampleObjectUpdate();

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void FirstName_WhenEmpty_ShouldPass_UnlikeCreate()
        {
            var model = new ExampleObjectUpdate { FirstName = "" };

            var result = _validator.TestValidate(model);

            // Update validator does NOT have NotEmpty, unlike Create
            result.ShouldNotHaveValidationErrorFor(x => x.FirstName);
        }

        [Theory]
        [InlineData("John123")]
        [InlineData("John Doe")]
        public void FirstName_WithNonAlphaChars_ShouldFail(string firstName)
        {
            var model = new ExampleObjectUpdate { FirstName = firstName };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.FirstName);
        }

        [Fact]
        public void Phone_WhenEmpty_ShouldPass_BecauseUpdateFieldsAreOptional()
        {
            var model = new ExampleObjectUpdate { Phone = "" };

            var result = _validator.TestValidate(model);

            // Update validator uses * quantifier — empty string is intentionally allowed
            result.ShouldNotHaveValidationErrorFor(x => x.Phone);
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("123-456")]
        public void Phone_WithNonNumericChars_ShouldFail(string phone)
        {
            var model = new ExampleObjectUpdate { Phone = phone };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Phone);
        }
    }
}
