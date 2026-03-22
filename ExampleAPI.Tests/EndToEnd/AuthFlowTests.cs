using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using ExampleAPI.Contracts.Shared;
using ExampleAPI.Helpers;
using ExampleAPI.Options;
using ExampleAPI.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace ExampleAPI.Tests.EndToEnd
{
    public class AuthFlowTests
    {
        private readonly JwtSettings _settings;
        private readonly TokenValidationParameters _tokenParams;
        private readonly JWTService _jwtService;
        private readonly Mock<RequestDelegate> _mockNext;

        public AuthFlowTests()
        {
            _settings = new JwtSettings
            {
                Secret = "ThisIsATestSecretKeyThatIsLongEnoughForHmacSha512Algorithm!!",
                Issuer = "TestIssuer",
                TokenLifetime = TimeSpan.FromHours(1)
            };

            _tokenParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret)),
                ValidateIssuer = true,
                ValidIssuer = _settings.Issuer,
                ValidateAudience = true,
                ValidAudience = _settings.Issuer,
                RequireExpirationTime = true,
                ValidateLifetime = true
            };

            _jwtService = new JWTService(_settings, _tokenParams);

            _mockNext = new Mock<RequestDelegate>();
            _mockNext.Setup(n => n(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);
        }

        [Fact]
        public async Task EndToEnd_GenerateToken_ThenMiddleware_ShouldExtractUserWithoutNullReference()
        {
            // Arrange: create a user and generate a JWT token
            var originalUserId = Guid.NewGuid();
            var originalUser = new LoggedinUser
            {
                Username = "e2euser",
                Email = "e2e@example.com",
                UserID = originalUserId
            };

            var token = _jwtService.GenerateJSONWebToken(originalUser);

            // Validate the token and extract the ClaimsPrincipal (simulates what ASP.NET auth does)
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, _tokenParams, out _);

            // Build an HttpContext with the validated principal (simulates the auth middleware pipeline)
            var context = new DefaultHttpContext();
            context.User = principal;
            context.Connection.RemoteIpAddress = IPAddress.Loopback;

            var middleware = new JWTHelper(_mockNext.Object);

            // Act: invoke the middleware — this is where NullReferenceException would occur
            // if UserID claim was missing
            Func<Task> act = () => middleware.Invoke(context, new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string> { { "BackendAuth:TokenPassword", "WhyHaveAStaticToken?" } }).Build());

            // Assert: no exception thrown
            await act.Should().NotThrowAsync<NullReferenceException>();

            // Assert: user was attached to context with correct values
            context.Items.Should().ContainKey("User");
            var extractedUser = context.Items["User"] as LoggedinUser;
            extractedUser.Should().NotBeNull();
            extractedUser.UserID.Should().Be(originalUserId);
            extractedUser.Username.Should().Be("e2euser");
            extractedUser.Email.Should().Be("e2e@example.com");
        }

        [Fact]
        public async Task EndToEnd_GenerateToken_ThenMiddleware_ShouldCallNextDelegate()
        {
            var user = new LoggedinUser
            {
                Username = "testuser",
                Email = "test@example.com",
                UserID = Guid.NewGuid()
            };

            var token = _jwtService.GenerateJSONWebToken(user);

            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, _tokenParams, out _);

            var context = new DefaultHttpContext();
            context.User = principal;
            context.Connection.RemoteIpAddress = IPAddress.Loopback;

            var middleware = new JWTHelper(_mockNext.Object);

            await middleware.Invoke(context, new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string> { { "BackendAuth:TokenPassword", "WhyHaveAStaticToken?" } }).Build());

            _mockNext.Verify(n => n(context), Times.Once);
        }
    }
}
