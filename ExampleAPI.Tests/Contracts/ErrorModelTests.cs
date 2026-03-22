using System.Text.Json;
using ExampleAPI.Contracts.Shared;
using FluentAssertions;
using Xunit;

namespace ExampleAPI.Tests.Contracts
{
    public class ErrorModelTests
    {
        [Fact]
        public void Properties_ShouldBeSettable()
        {
            var model = new ErrorModel
            {
                FieldName = "Username",
                Message = "Username is required"
            };

            model.FieldName.Should().Be("Username");
            model.Message.Should().Be("Username is required");
        }

        [Fact]
        public void Properties_DefaultValues_ShouldBeNull()
        {
            var model = new ErrorModel();

            model.FieldName.Should().BeNull();
            model.Message.Should().BeNull();
        }

        [Fact]
        public void ShouldHaveTwoProperties()
        {
            var properties = typeof(ErrorModel).GetProperties();

            properties.Should().HaveCount(2);
        }

        [Fact]
        public void Properties_ShouldBeStringType()
        {
            var properties = typeof(ErrorModel).GetProperties();

            foreach (var prop in properties)
            {
                prop.PropertyType.Should().Be(typeof(string));
            }
        }

        [Fact]
        public void ShouldSerializeToJson()
        {
            var model = new ErrorModel
            {
                FieldName = "Email",
                Message = "Invalid email format"
            };

            var json = JsonSerializer.Serialize(model);
            var deserialized = JsonSerializer.Deserialize<ErrorModel>(json);

            deserialized.FieldName.Should().Be("Email");
            deserialized.Message.Should().Be("Invalid email format");
        }

        [Fact]
        public void ShouldDeserializeFromJson()
        {
            var json = "{\"FieldName\":\"Password\",\"Message\":\"Too short\"}";

            var model = JsonSerializer.Deserialize<ErrorModel>(json);

            model.FieldName.Should().Be("Password");
            model.Message.Should().Be("Too short");
        }
    }
}
