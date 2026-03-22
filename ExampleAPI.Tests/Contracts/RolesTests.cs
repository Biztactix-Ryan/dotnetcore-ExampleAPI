using ExampleAPI.Contracts.Shared;
using FluentAssertions;
using Xunit;

namespace ExampleAPI.Tests.Contracts
{
    public class RolesTests
    {
        [Fact]
        public void User_ShouldBeCorrectValue()
        {
            Roles.User.Should().Be("User");
        }

        [Fact]
        public void Backend_ShouldBeCorrectValue()
        {
            Roles.Backend.Should().Be("Backend");
        }

        [Fact]
        public void Any_ShouldBeCorrectValue()
        {
            Roles.Any.Should().Be("???");
        }

        [Fact]
        public void RoleValues_ShouldAllBeDistinct()
        {
            var roles = new[] { Roles.User, Roles.Backend, Roles.Any };
            roles.Should().OnlyHaveUniqueItems();
        }
    }
}
