using ExampleAPI.Contracts.Shared;
using FluentAssertions;
using Xunit;

namespace ExampleAPI.Tests.Contracts
{
    public class LoggedinUserTests
    {
        [Fact]
        public void Properties_ShouldBeSettable()
        {
            var userId = Guid.NewGuid();
            var user = new LoggedinUser
            {
                Username = "testuser",
                Email = "test@example.com",
                UserID = userId
            };

            user.Username.Should().Be("testuser");
            user.Email.Should().Be("test@example.com");
            user.UserID.Should().Be(userId);
        }

        [Fact]
        public void UserID_DefaultValue_ShouldBeEmptyGuid()
        {
            var user = new LoggedinUser();

            user.UserID.Should().Be(Guid.Empty);
        }
    }
}
