using System.Text.Json;
using ExampleAPI.Contracts.V2.Auth;
using FluentAssertions;
using Xunit;

namespace ExampleAPI.Tests.Contracts.V2
{
    public class APIKeyAuthenticateRequestTests
    {
        [Fact]
        public void Properties_ShouldBeSettable()
        {
            var request = new APIKeyAuthenticateRequest
            {
                APIKey = "test-key",
                APIPass = "test-pass",
                RefreshToken = "test-refresh"
            };

            request.APIKey.Should().Be("test-key");
            request.APIPass.Should().Be("test-pass");
            request.RefreshToken.Should().Be("test-refresh");
        }

        [Fact]
        public void Properties_DefaultValues_ShouldBeNull()
        {
            var request = new APIKeyAuthenticateRequest();

            request.APIKey.Should().BeNull();
            request.APIPass.Should().BeNull();
            request.RefreshToken.Should().BeNull();
        }

        [Fact]
        public void ShouldHaveThreeProperties()
        {
            var properties = typeof(APIKeyAuthenticateRequest).GetProperties();

            properties.Should().HaveCount(3);
        }

        [Fact]
        public void Properties_ShouldBeStringType()
        {
            var properties = typeof(APIKeyAuthenticateRequest).GetProperties();

            foreach (var prop in properties)
            {
                prop.PropertyType.Should().Be(typeof(string));
            }
        }

        [Fact]
        public void ShouldSerializeToJson()
        {
            var request = new APIKeyAuthenticateRequest
            {
                APIKey = "key1",
                APIPass = "pass1",
                RefreshToken = "refresh1"
            };

            var json = JsonSerializer.Serialize(request);
            var deserialized = JsonSerializer.Deserialize<APIKeyAuthenticateRequest>(json);

            deserialized.APIKey.Should().Be("key1");
            deserialized.APIPass.Should().Be("pass1");
            deserialized.RefreshToken.Should().Be("refresh1");
        }
    }
}
