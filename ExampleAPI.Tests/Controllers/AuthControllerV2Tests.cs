using ExampleAPI.Contracts.V2;
using ExampleAPI.Contracts.V2.Auth;
using ExampleAPI.Controllers.V2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ExampleAPI.Tests.Controllers
{
    public class AuthControllerV2EndpointTests
    {
        private readonly AuthController _controller = new();

        // --- UserLogin endpoint tests ---

        [Fact]
        public void Post_UserLogin_WithValidCredentials_ShouldReturnOk()
        {
            var request = new UserAuthenticateRequest { User = "testuser", Pass = "password123" };

            var result = _controller.Post(request);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void Post_UserLogin_WithValidCredentials_ShouldReturnGoTeam()
        {
            var request = new UserAuthenticateRequest { User = "testuser", Pass = "password123" };

            var result = _controller.Post(request) as OkObjectResult;

            result.Should().NotBeNull();
            result!.Value.Should().Be("GoTeam!");
        }

        [Fact]
        public void Post_UserLogin_WithEmptyUser_ShouldStillReturnOk()
        {
            var request = new UserAuthenticateRequest { User = "", Pass = "password123" };

            var result = _controller.Post(request);

            // Controller does not validate — validation is handled by the filter/validator
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void Post_UserLogin_WithEmptyPass_ShouldStillReturnOk()
        {
            var request = new UserAuthenticateRequest { User = "testuser", Pass = "" };

            var result = _controller.Post(request);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void Post_UserLogin_WithNullFields_ShouldStillReturnOk()
        {
            var request = new UserAuthenticateRequest { User = null, Pass = null };

            var result = _controller.Post(request);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void Post_UserLogin_ShouldReturnStatusCode200()
        {
            var request = new UserAuthenticateRequest { User = "admin", Pass = "secret" };

            var result = _controller.Post(request) as OkObjectResult;

            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(200);
        }

        // --- APILogin endpoint tests ---

        [Fact]
        public void Post_APILogin_WithValidCredentials_ShouldReturnOk()
        {
            var request = new APIKeyAuthenticateRequest
            {
                APIKey = "my-api-key",
                APIPass = "my-api-pass",
                RefreshToken = "my-refresh-token"
            };

            var result = _controller.Post(request);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void Post_APILogin_WithValidCredentials_ShouldReturnGoTeam()
        {
            var request = new APIKeyAuthenticateRequest
            {
                APIKey = "my-api-key",
                APIPass = "my-api-pass",
                RefreshToken = "my-refresh-token"
            };

            var result = _controller.Post(request) as OkObjectResult;

            result.Should().NotBeNull();
            result!.Value.Should().Be("GoTeam!");
        }

        [Fact]
        public void Post_APILogin_WithNullRefreshToken_ShouldStillReturnOk()
        {
            var request = new APIKeyAuthenticateRequest
            {
                APIKey = "my-api-key",
                APIPass = "my-api-pass",
                RefreshToken = null
            };

            var result = _controller.Post(request);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void Post_APILogin_WithEmptyFields_ShouldStillReturnOk()
        {
            var request = new APIKeyAuthenticateRequest
            {
                APIKey = "",
                APIPass = "",
                RefreshToken = ""
            };

            var result = _controller.Post(request);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void Post_APILogin_WithNullFields_ShouldStillReturnOk()
        {
            var request = new APIKeyAuthenticateRequest
            {
                APIKey = null,
                APIPass = null,
                RefreshToken = null
            };

            var result = _controller.Post(request);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void Post_APILogin_ShouldReturnStatusCode200()
        {
            var request = new APIKeyAuthenticateRequest
            {
                APIKey = "key",
                APIPass = "pass",
                RefreshToken = "token"
            };

            var result = _controller.Post(request) as OkObjectResult;

            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(200);
        }

        // --- Response type consistency tests ---

        [Fact]
        public void Post_UserLogin_ResultShouldBeIActionResult()
        {
            var request = new UserAuthenticateRequest { User = "user", Pass = "pass" };

            var result = _controller.Post(request);

            result.Should().BeAssignableTo<IActionResult>();
        }

        [Fact]
        public void Post_APILogin_ResultShouldBeIActionResult()
        {
            var request = new APIKeyAuthenticateRequest
            {
                APIKey = "key",
                APIPass = "pass",
                RefreshToken = "token"
            };

            var result = _controller.Post(request);

            result.Should().BeAssignableTo<IActionResult>();
        }

        // --- Controller attribute tests ---

        [Fact]
        public void Controller_ShouldHaveApiControllerAttribute()
        {
            var attributes = typeof(AuthController).GetCustomAttributes(typeof(ApiControllerAttribute), true);

            attributes.Should().HaveCount(1);
        }

        [Fact]
        public void Controller_ShouldInheritFromControllerBase()
        {
            typeof(AuthController).Should().BeDerivedFrom<ControllerBase>();
        }

        [Fact]
        public void UserLogin_ShouldHaveHttpPostAttribute()
        {
            var method = typeof(AuthController).GetMethod(nameof(AuthController.Post),
                new[] { typeof(UserAuthenticateRequest) });

            method.Should().NotBeNull();
            var attributes = method!.GetCustomAttributes(typeof(HttpPostAttribute), true);
            attributes.Should().HaveCount(1);
        }

        [Fact]
        public void APILogin_ShouldHaveHttpPostAttribute()
        {
            var method = typeof(AuthController).GetMethod(nameof(AuthController.Post),
                new[] { typeof(APIKeyAuthenticateRequest) });

            method.Should().NotBeNull();
            var attributes = method!.GetCustomAttributes(typeof(HttpPostAttribute), true);
            attributes.Should().HaveCount(1);
        }

        [Fact]
        public void UserLogin_ShouldUseCorrectRoute()
        {
            var method = typeof(AuthController).GetMethod(nameof(AuthController.Post),
                new[] { typeof(UserAuthenticateRequest) });
            var attribute = method!.GetCustomAttributes(typeof(HttpPostAttribute), true)
                .Cast<HttpPostAttribute>().First();

            attribute.Template.Should().Be("api/v2/auth/UserLogin");
        }

        [Fact]
        public void APILogin_ShouldUseCorrectRoute()
        {
            var method = typeof(AuthController).GetMethod(nameof(AuthController.Post),
                new[] { typeof(APIKeyAuthenticateRequest) });
            var attribute = method!.GetCustomAttributes(typeof(HttpPostAttribute), true)
                .Cast<HttpPostAttribute>().First();

            attribute.Template.Should().Be("api/v2/auth/APILogin");
        }
    }
}
