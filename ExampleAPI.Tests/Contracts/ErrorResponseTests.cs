using ExampleAPI.Contracts.Shared;
using FluentAssertions;
using Xunit;

namespace ExampleAPI.Tests.Contracts
{
    public class ErrorResponseTests
    {
        [Fact]
        public void DefaultConstructor_ShouldInitializeEmptyErrorsList()
        {
            var response = new ErrorResponse();

            response.Errors.Should().NotBeNull();
            response.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Constructor_WithErrorModel_ShouldAddToList()
        {
            var error = new ErrorModel
            {
                FieldName = "Email",
                Message = "Email is required"
            };

            var response = new ErrorResponse(error);

            response.Errors.Should().HaveCount(1);
            response.Errors[0].FieldName.Should().Be("Email");
            response.Errors[0].Message.Should().Be("Email is required");
        }

        [Fact]
        public void Errors_ShouldAllowMultipleErrors()
        {
            var response = new ErrorResponse();
            response.Errors.Add(new ErrorModel { FieldName = "A", Message = "Error A" });
            response.Errors.Add(new ErrorModel { FieldName = "B", Message = "Error B" });

            response.Errors.Should().HaveCount(2);
        }
    }
}
