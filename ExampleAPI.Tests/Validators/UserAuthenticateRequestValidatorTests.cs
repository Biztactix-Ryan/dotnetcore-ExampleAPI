using ExampleAPI.Contracts.V1;
using ExampleAPI.Contracts.V1.Auth;
using ExampleAPI.Contracts.V1.Validators;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace ExampleAPI.Tests.Validators
{
    public class UserAuthenticateRequestValidatorTests
    {
        private readonly UserAuthenticateRequestValidator _validator = new();

        [Fact]
        public void Valid_Request_ShouldPass()
        {
            var model = new UserAuthenticateRequest
            {
                User = "testuser",
                Pass = "password123"
            };

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void User_WhenEmpty_ShouldFail(string user)
        {
            var model = new UserAuthenticateRequest { User = user, Pass = "password" };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.User);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Pass_WhenEmpty_ShouldFail(string pass)
        {
            var model = new UserAuthenticateRequest { User = "testuser", Pass = pass };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Pass);
        }

        [Theory]
        [InlineData("user@name")]
        [InlineData("user name")]
        [InlineData("user!name")]
        [InlineData("user.name")]
        [InlineData("user-name")]
        public void User_WithSpecialChars_ShouldFail(string user)
        {
            var model = new UserAuthenticateRequest { User = user, Pass = "password" };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.User);
        }

        [Fact]
        public void User_WithAlphanumericOnly_ShouldPass()
        {
            var model = new UserAuthenticateRequest { User = "TestUser123", Pass = "password" };

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.User);
        }

        [Fact]
        public void Pass_WithSpecialChars_ShouldPass()
        {
            // Pass has no regex constraint, only NotEmpty
            var model = new UserAuthenticateRequest
            {
                User = "testuser",
                Pass = "p@$$w0rd!#%"
            };

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
