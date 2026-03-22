using ExampleAPI.Contracts.Shared;
using ExampleAPI.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Net;
using System.Security.Claims;
using Xunit;

namespace ExampleAPI.Tests.Middleware
{
    public class JWTHelperTests
    {
        private readonly Mock<RequestDelegate> _mockNext;
        private readonly IConfiguration _configuration;

        public JWTHelperTests()
        {
            _mockNext = new Mock<RequestDelegate>();
            _mockNext.Setup(n => n(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "BackendAuth:TokenPassword", "WhyHaveAStaticToken?" }
                })
                .Build();
        }

        private DefaultHttpContext CreateAuthenticatedContext(string username, string email, string userId)
        {
            var claims = new List<Claim>
            {
                new Claim("Username", username),
                new Claim("Email", email),
                new Claim("UserID", userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            var context = new DefaultHttpContext();
            context.User = principal;
            context.Connection.RemoteIpAddress = IPAddress.Loopback;
            return context;
        }

        private DefaultHttpContext CreateUnauthenticatedContext()
        {
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity());
            context.Connection.RemoteIpAddress = IPAddress.Loopback;
            return context;
        }

        [Fact]
        public async Task Invoke_Unauthenticated_ShouldCallNext()
        {
            var middleware = new JWTHelper(_mockNext.Object);
            var context = CreateUnauthenticatedContext();

            await middleware.Invoke(context, _configuration);

            _mockNext.Verify(n => n(context), Times.Once);
        }

        [Fact]
        public async Task Invoke_Unauthenticated_ShouldNotAttachUser()
        {
            var middleware = new JWTHelper(_mockNext.Object);
            var context = CreateUnauthenticatedContext();

            await middleware.Invoke(context, _configuration);

            context.Items.Should().NotContainKey("User");
        }

        [Fact]
        public async Task Invoke_Authenticated_ShouldCallNext()
        {
            var middleware = new JWTHelper(_mockNext.Object);
            var userId = Guid.NewGuid().ToString();
            var context = CreateAuthenticatedContext("testuser", "test@test.com", userId);

            await middleware.Invoke(context, _configuration);

            _mockNext.Verify(n => n(context), Times.Once);
        }

        [Fact]
        public async Task Invoke_Authenticated_ShouldAttachUserToContext()
        {
            var middleware = new JWTHelper(_mockNext.Object);
            var userId = Guid.NewGuid();
            var context = CreateAuthenticatedContext("testuser", "test@test.com", userId.ToString());

            await middleware.Invoke(context, _configuration);

            context.Items.Should().ContainKey("User");
            var user = context.Items["User"] as LoggedinUser;
            user.Should().NotBeNull();
            user.Username.Should().Be("testuser");
            user.Email.Should().Be("test@test.com");
            user.UserID.Should().Be(userId);
        }

        [Fact]
        public async Task Invoke_Authenticated_ShouldSetBackendAuthFalseByDefault()
        {
            var middleware = new JWTHelper(_mockNext.Object);
            var userId = Guid.NewGuid().ToString();
            var context = CreateAuthenticatedContext("testuser", "test@test.com", userId);

            await middleware.Invoke(context, _configuration);

            context.Items.Should().ContainKey("BackendAuth");
            context.Items["BackendAuth"].Should().Be(false);
        }

        [Fact]
        public async Task Invoke_Authenticated_ShouldSafelyParseValidUserIdWithTryParse()
        {
            var middleware = new JWTHelper(_mockNext.Object);
            var expectedId = Guid.NewGuid();
            var context = CreateAuthenticatedContext("testuser", "test@test.com", expectedId.ToString());

            await middleware.Invoke(context, _configuration);

            context.Items.Should().ContainKey("User");
            var user = context.Items["User"] as LoggedinUser;
            user.Should().NotBeNull();
            user.UserID.Should().Be(expectedId);
        }

        [Theory]
        [InlineData("not-a-guid")]
        [InlineData("")]
        [InlineData("12345")]
        public async Task Invoke_Authenticated_ShouldNotAttachUserWhenUserIdIsInvalid(string invalidUserId)
        {
            var claims = new List<Claim>
            {
                new Claim("Username", "testuser"),
                new Claim("Email", "test@test.com"),
                new Claim("UserID", invalidUserId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(identity);
            context.Connection.RemoteIpAddress = IPAddress.Loopback;

            var middleware = new JWTHelper(_mockNext.Object);

            await middleware.Invoke(context, _configuration);

            context.Items.Should().NotContainKey("User");
        }

        [Fact]
        public async Task Invoke_AuthenticatedWithInvalidGuid_ShouldStillCallNext()
        {
            var claims = new List<Claim>
            {
                new Claim("Username", "testuser"),
                new Claim("Email", "test@test.com"),
                new Claim("UserID", "not-a-guid")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(identity);
            context.Connection.RemoteIpAddress = IPAddress.Loopback;

            var middleware = new JWTHelper(_mockNext.Object);

            await middleware.Invoke(context, _configuration);

            _mockNext.Verify(n => n(context), Times.Once);
        }

        [Fact]
        public async Task Invoke_AuthenticatedWithMissingClaims_ShouldStillCallNext()
        {
            var identity = new ClaimsIdentity(new List<Claim>(), "TestAuth");
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(identity);
            context.Connection.RemoteIpAddress = IPAddress.Loopback;

            var middleware = new JWTHelper(_mockNext.Object);

            await middleware.Invoke(context, _configuration);

            _mockNext.Verify(n => n(context), Times.Once);
        }

        [Fact]
        public async Task Invoke_Authenticated_MissingUserIdClaim_ShouldNotThrow()
        {
            var claims = new List<Claim>
            {
                new Claim("Username", "testuser"),
                new Claim("Email", "test@test.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(identity);
            context.Connection.RemoteIpAddress = IPAddress.Loopback;

            var middleware = new JWTHelper(_mockNext.Object);

            var act = () => middleware.Invoke(context, _configuration);

            await act.Should().NotThrowAsync();
            context.Items.Should().NotContainKey("User");
            _mockNext.Verify(n => n(context), Times.Once);
        }

        [Fact]
        public async Task Invoke_Authenticated_MissingUserIdClaim_FailsGracefully()
        {
            var claims = new List<Claim>
            {
                new Claim("Username", "testuser"),
                new Claim("Email", "test@test.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(identity);
            context.Connection.RemoteIpAddress = IPAddress.Loopback;

            var middleware = new JWTHelper(_mockNext.Object);

            var act = () => middleware.Invoke(context, _configuration);

            await act.Should().NotThrowAsync();
            context.Items.Should().NotContainKey("User");
            _mockNext.Verify(n => n(context), Times.Once);
        }

        [Theory]
        [InlineData("not-a-guid")]
        [InlineData("")]
        [InlineData("12345")]
        [InlineData("zzzzzzzz-zzzz-zzzz-zzzz-zzzzzzzzzzzz")]
        public async Task Invoke_Authenticated_InvalidUserIdClaim_FailsGracefully(string invalidUserId)
        {
            var claims = new List<Claim>
            {
                new Claim("Username", "testuser"),
                new Claim("Email", "test@test.com"),
                new Claim("UserID", invalidUserId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(identity);
            context.Connection.RemoteIpAddress = IPAddress.Loopback;

            var middleware = new JWTHelper(_mockNext.Object);

            var act = () => middleware.Invoke(context, _configuration);

            await act.Should().NotThrowAsync();
            context.Items.Should().NotContainKey("User");
            _mockNext.Verify(n => n(context), Times.Once);
        }

        [Fact]
        public async Task Invoke_AuthenticatedWithBackendClaim_ShouldVerifyBackendToken()
        {
            var claims = new List<Claim>
            {
                new Claim("Username", "backend-app"),
                new Claim("Email", "backend@test.com"),
                new Claim("UserID", Guid.NewGuid().ToString()),
                new Claim("BACKEND", "TRUE"),
                new Claim("Token", Convert.ToBase64String(new byte[16])),
                new Claim("Check", "invalid-hash"),
                new Claim("AppName", "TestApp")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(identity);
            context.Connection.RemoteIpAddress = IPAddress.Loopback;

            var middleware = new JWTHelper(_mockNext.Object);

            await middleware.Invoke(context, _configuration);

            context.Items.Should().ContainKey("BackendAuth");
        }

        // --- US-EX-21-2: Missing Token/Check/AppName claims don't crash the middleware ---

        [Fact]
        public async Task Invoke_BackendTrue_MissingTokenClaim_ShouldNotCrashMiddleware()
        {
            var claims = new List<Claim>
            {
                new Claim("Username", "backend-app"),
                new Claim("Email", "backend@test.com"),
                new Claim("UserID", Guid.NewGuid().ToString()),
                new Claim("BACKEND", "TRUE"),
                new Claim("Check", "somehash"),
                new Claim("AppName", "TestApp")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(identity);
            context.Connection.RemoteIpAddress = IPAddress.Loopback;

            var middleware = new JWTHelper(_mockNext.Object);

            var act = () => middleware.Invoke(context, _configuration);

            await act.Should().NotThrowAsync();
            context.Items["BackendAuth"].Should().Be(false);
            _mockNext.Verify(n => n(context), Times.Once);
        }

        [Fact]
        public async Task Invoke_BackendTrue_MissingCheckClaim_ShouldNotCrashMiddleware()
        {
            var claims = new List<Claim>
            {
                new Claim("Username", "backend-app"),
                new Claim("Email", "backend@test.com"),
                new Claim("UserID", Guid.NewGuid().ToString()),
                new Claim("BACKEND", "TRUE"),
                new Claim("Token", Convert.ToBase64String(new byte[16])),
                new Claim("AppName", "TestApp")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(identity);
            context.Connection.RemoteIpAddress = IPAddress.Loopback;

            var middleware = new JWTHelper(_mockNext.Object);

            var act = () => middleware.Invoke(context, _configuration);

            await act.Should().NotThrowAsync();
            context.Items["BackendAuth"].Should().Be(false);
            _mockNext.Verify(n => n(context), Times.Once);
        }

        [Fact]
        public async Task Invoke_BackendTrue_MissingAppNameClaim_ShouldNotCrashMiddleware()
        {
            var claims = new List<Claim>
            {
                new Claim("Username", "backend-app"),
                new Claim("Email", "backend@test.com"),
                new Claim("UserID", Guid.NewGuid().ToString()),
                new Claim("BACKEND", "TRUE"),
                new Claim("Token", Convert.ToBase64String(new byte[16])),
                new Claim("Check", "somehash")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(identity);
            context.Connection.RemoteIpAddress = IPAddress.Loopback;

            var middleware = new JWTHelper(_mockNext.Object);

            var act = () => middleware.Invoke(context, _configuration);

            await act.Should().NotThrowAsync();
            context.Items["BackendAuth"].Should().Be(false);
            _mockNext.Verify(n => n(context), Times.Once);
        }

        [Fact]
        public async Task Invoke_BackendTrue_AllThreeClaimsMissing_ShouldNotCrashMiddleware()
        {
            var claims = new List<Claim>
            {
                new Claim("Username", "backend-app"),
                new Claim("Email", "backend@test.com"),
                new Claim("UserID", Guid.NewGuid().ToString()),
                new Claim("BACKEND", "TRUE")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(identity);
            context.Connection.RemoteIpAddress = IPAddress.Loopback;

            var middleware = new JWTHelper(_mockNext.Object);

            var act = () => middleware.Invoke(context, _configuration);

            await act.Should().NotThrowAsync();
            context.Items["BackendAuth"].Should().Be(false);
            _mockNext.Verify(n => n(context), Times.Once);
        }
    }
}
