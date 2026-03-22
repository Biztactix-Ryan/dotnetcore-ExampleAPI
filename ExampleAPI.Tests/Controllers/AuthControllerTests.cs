using ExampleAPI.Contracts.V1;
using ExampleAPI.Contracts.V1.Auth;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ExampleAPI.Tests.Controllers
{
    public class AuthControllerV1Tests
    {
        private readonly ExampleAPI.Controllers.V1.AuthController _controller = new();

        [Fact]
        public void Post_UserLogin_ShouldReturnOk()
        {
            var request = new UserAuthenticateRequest { User = "test", Pass = "pass" };

            var result = _controller.Post(request);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void Post_UserLogin_ShouldReturnGoTeam()
        {
            var request = new UserAuthenticateRequest { User = "test", Pass = "pass" };

            var result = _controller.Post(request) as OkObjectResult;

            result.Value.Should().Be("GoTeam!");
        }

        [Fact]
        public void Post_APILogin_ShouldReturnOk()
        {
            var request = new APIKeyAuthenticateRequest
            {
                APIKey = "key",
                APIPass = "pass",
                RefreshToken = "token"
            };

            var result = _controller.Post(request);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void Post_APILogin_ShouldReturnGoTeam()
        {
            var request = new APIKeyAuthenticateRequest
            {
                APIKey = "key",
                APIPass = "pass",
                RefreshToken = "token"
            };

            var result = _controller.Post(request) as OkObjectResult;

            result.Value.Should().Be("GoTeam!");
        }
    }

    public class AuthControllerV2Tests
    {
        private readonly ExampleAPI.Controllers.V2.AuthController _controller = new();

        [Fact]
        public void Post_UserLogin_ShouldReturnOk()
        {
            var request = new ExampleAPI.Contracts.V2.UserAuthenticateRequest
            {
                User = "test",
                Pass = "pass"
            };

            var result = _controller.Post(request);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void Post_APILogin_ShouldReturnOk()
        {
            var request = new ExampleAPI.Contracts.V2.Auth.APIKeyAuthenticateRequest
            {
                APIKey = "key",
                APIPass = "pass",
                RefreshToken = "token"
            };

            var result = _controller.Post(request);

            result.Should().BeOfType<OkObjectResult>();
        }
    }
}
