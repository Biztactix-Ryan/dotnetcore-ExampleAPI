using System.Text.Json;
using ExampleAPI.Contracts.V1;
using FluentAssertions;
using Xunit;

namespace ExampleAPI.Tests.Contracts.V1
{
    public class UserAuthenticateRequestTests
    {
        [Fact]
        public void Properties_ShouldBeSettable()
        {
            var request = new UserAuthenticateRequest
            {
                User = "testuser",
                Pass = "testpass"
            };

            request.User.Should().Be("testuser");
            request.Pass.Should().Be("testpass");
        }

        [Fact]
        public void Properties_DefaultValues_ShouldBeNull()
        {
            var request = new UserAuthenticateRequest();

            request.User.Should().BeNull();
            request.Pass.Should().BeNull();
        }

        [Fact]
        public void ShouldHaveTwoProperties()
        {
            var properties = typeof(UserAuthenticateRequest).GetProperties();

            properties.Should().HaveCount(2);
        }

        [Fact]
        public void Properties_ShouldBeStringType()
        {
            var properties = typeof(UserAuthenticateRequest).GetProperties();

            foreach (var prop in properties)
            {
                prop.PropertyType.Should().Be(typeof(string));
            }
        }

        [Fact]
        public void ShouldSerializeToJson()
        {
            var request = new UserAuthenticateRequest
            {
                User = "user1",
                Pass = "pass1"
            };

            var json = JsonSerializer.Serialize(request);
            var deserialized = JsonSerializer.Deserialize<UserAuthenticateRequest>(json);

            deserialized.User.Should().Be("user1");
            deserialized.Pass.Should().Be("pass1");
        }
    }
}
