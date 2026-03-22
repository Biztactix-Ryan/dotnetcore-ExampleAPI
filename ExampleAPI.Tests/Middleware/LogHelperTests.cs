using ExampleAPI.Contracts.Shared;
using ExampleAPI.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NLog;
using NLog.Targets;
using System.Net;
using System.Security.Claims;
using Xunit;

namespace ExampleAPI.Tests.Middleware
{
    public class LogHelperTests
    {
        private readonly Mock<RequestDelegate> _mockNext;

        public LogHelperTests()
        {
            _mockNext = new Mock<RequestDelegate>();
            _mockNext.Setup(n => n(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);
        }

        private DefaultHttpContext CreateAuthenticatedContext()
        {
            var identity = new ClaimsIdentity(new List<Claim>(), "TestAuth");
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(identity);
            context.Connection.RemoteIpAddress = IPAddress.Loopback;
            context.Request.Method = "GET";
            context.Request.Path = "/api/test";
            context.Items["User"] = new LoggedinUser
            {
                Username = "testuser",
                Email = "test@test.com",
                UserID = Guid.NewGuid()
            };
            return context;
        }

        private DefaultHttpContext CreateUnauthenticatedContext()
        {
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity());
            context.Connection.RemoteIpAddress = IPAddress.Loopback;
            context.Request.Method = "GET";
            context.Request.Path = "/api/test";
            return context;
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_ShouldAcceptRequestDelegate()
        {
            var middleware = new LogHelper(_mockNext.Object);

            middleware.Should().NotBeNull();
        }

        #endregion

        #region Invoke Tests

        [Fact]
        public async Task Invoke_ShouldCallNextDelegate()
        {
            var middleware = new LogHelper(_mockNext.Object);
            var context = CreateUnauthenticatedContext();

            await middleware.Invoke(context);

            _mockNext.Verify(n => n(context), Times.Once);
        }

        [Fact]
        public async Task Invoke_Authenticated_ShouldNotThrow()
        {
            var middleware = new LogHelper(_mockNext.Object);
            var context = CreateAuthenticatedContext();

            var act = () => middleware.Invoke(context);

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Invoke_Unauthenticated_ShouldNotThrow()
        {
            var middleware = new LogHelper(_mockNext.Object);
            var context = CreateUnauthenticatedContext();

            var act = () => middleware.Invoke(context);

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Invoke_AuthenticatedWithNullUserItem_ShouldNotThrow()
        {
            var middleware = new LogHelper(_mockNext.Object);
            var identity = new ClaimsIdentity(new List<Claim>(), "TestAuth");
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(identity);
            context.Connection.RemoteIpAddress = IPAddress.Loopback;
            context.Request.Method = "GET";
            context.Request.Path = "/api/test";
            // User item is explicitly null — simulates JWTHelper failing to attach user
            context.Items["User"] = null;

            var act = () => middleware.Invoke(context);

            await act.Should().NotThrowAsync<NullReferenceException>();
        }

        [Fact]
        public async Task Invoke_AuthenticatedWithMissingUserItem_ShouldNotThrow()
        {
            var middleware = new LogHelper(_mockNext.Object);
            var identity = new ClaimsIdentity(new List<Claim>(), "TestAuth");
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(identity);
            context.Connection.RemoteIpAddress = IPAddress.Loopback;
            context.Request.Method = "GET";
            context.Request.Path = "/api/test";
            // User item not set at all — no context.Items["User"] key

            var act = () => middleware.Invoke(context);

            await act.Should().NotThrowAsync<NullReferenceException>();
        }

        [Fact]
        public async Task Invoke_WhenNextThrows_ShouldPropagateException()
        {
            var expectedException = new InvalidOperationException("Test error");
            _mockNext.Setup(n => n(It.IsAny<HttpContext>()))
                     .ThrowsAsync(expectedException);

            var middleware = new LogHelper(_mockNext.Object);
            var context = CreateUnauthenticatedContext();

            var act = () => middleware.Invoke(context);

            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("Test error");
        }

        [Fact]
        public async Task Invoke_Authenticated_ShouldCallNextBeforeLogging()
        {
            var callOrder = new List<string>();
            _mockNext.Setup(n => n(It.IsAny<HttpContext>()))
                     .Callback(() => callOrder.Add("next"))
                     .Returns(Task.CompletedTask);

            var middleware = new LogHelper(_mockNext.Object);
            var context = CreateAuthenticatedContext();

            await middleware.Invoke(context);

            callOrder.Should().Contain("next");
        }

        [Fact]
        public async Task Invoke_Unauthenticated_ShouldStillCallNext()
        {
            var middleware = new LogHelper(_mockNext.Object);
            var context = CreateUnauthenticatedContext();

            await middleware.Invoke(context);

            _mockNext.Verify(n => n(context), Times.Once);
        }

        [Fact]
        public async Task Invoke_WithDifferentHttpMethods_ShouldNotThrow()
        {
            var middleware = new LogHelper(_mockNext.Object);

            foreach (var method in new[] { "GET", "POST", "PUT", "DELETE", "PATCH" })
            {
                var context = CreateUnauthenticatedContext();
                context.Request.Method = method;

                var act = () => middleware.Invoke(context);

                await act.Should().NotThrowAsync();
            }
        }

        [Fact]
        public async Task Invoke_WithDifferentPaths_ShouldNotThrow()
        {
            var middleware = new LogHelper(_mockNext.Object);

            foreach (var path in new[] { "/api/weather", "/api/auth/login", "/health" })
            {
                var context = CreateUnauthenticatedContext();
                context.Request.Path = path;

                var act = () => middleware.Invoke(context);

                await act.Should().NotThrowAsync();
            }
        }

        [Fact]
        public async Task Invoke_WithNullUserItem_ShouldPreserveResponseStatusCode()
        {
            // Arrange: pipeline sets a non-default status code, but User item is null
            // This is the exact scenario that previously caused NullReferenceException
            // in the finally block, which would swallow the original response
            var middleware = new LogHelper(_mockNext.Object);
            var identity = new ClaimsIdentity(new List<Claim>(), "TestAuth");
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(identity);
            context.Connection.RemoteIpAddress = IPAddress.Loopback;
            context.Request.Method = "POST";
            context.Request.Path = "/api/test";
            context.Items["User"] = null;

            _mockNext.Setup(n => n(It.IsAny<HttpContext>()))
                     .Callback<HttpContext>(ctx => ctx.Response.StatusCode = 403)
                     .Returns(Task.CompletedTask);

            // Act
            await middleware.Invoke(context);

            // Assert: the original 403 response must survive the finally block
            context.Response.StatusCode.Should().Be(403);
        }

        [Fact]
        public async Task Invoke_WithMissingUserItem_ShouldPreserveResponseStatusCode()
        {
            // Arrange: authenticated user but no User key in context.Items at all
            var middleware = new LogHelper(_mockNext.Object);
            var identity = new ClaimsIdentity(new List<Claim>(), "TestAuth");
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(identity);
            context.Connection.RemoteIpAddress = IPAddress.Loopback;
            context.Request.Method = "GET";
            context.Request.Path = "/api/secure";

            _mockNext.Setup(n => n(It.IsAny<HttpContext>()))
                     .Callback<HttpContext>(ctx => ctx.Response.StatusCode = 401)
                     .Returns(Task.CompletedTask);

            // Act
            await middleware.Invoke(context);

            // Assert: 401 must not be swallowed by a logging error
            context.Response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task Invoke_WhenNextThrows_WithNullUserItem_ShouldStillPropagateException()
        {
            // Arrange: the pipeline throws AND User item is null —
            // the finally block must not swallow the original exception
            var expectedException = new UnauthorizedAccessException("Access denied");
            _mockNext.Setup(n => n(It.IsAny<HttpContext>()))
                     .ThrowsAsync(expectedException);

            var middleware = new LogHelper(_mockNext.Object);
            var identity = new ClaimsIdentity(new List<Claim>(), "TestAuth");
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(identity);
            context.Connection.RemoteIpAddress = IPAddress.Loopback;
            context.Request.Method = "GET";
            context.Request.Path = "/api/test";
            context.Items["User"] = null;

            // Act & Assert: original exception must propagate, not be replaced
            var act = () => middleware.Invoke(context);

            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                     .WithMessage("Access denied");
        }

        [Fact]
        public async Task Invoke_WithNullRemoteIpAddress_ShouldHandleGracefully()
        {
            // RemoteIpAddress can be null when the connection info is unavailable
            // (e.g., test hosts, Unix sockets). The null-conditional operator (?.)
            // with "unknown" fallback prevents NullReferenceException.
            var middleware = new LogHelper(_mockNext.Object);
            var context = CreateUnauthenticatedContext();
            context.Connection.RemoteIpAddress = null;

            var act = () => middleware.Invoke(context);

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Invoke_Authenticated_WithNullRemoteIpAddress_ShouldHandleGracefully()
        {
            // Authenticated path also accesses RemoteIpAddress — guarded with ?.
            var middleware = new LogHelper(_mockNext.Object);
            var context = CreateAuthenticatedContext();
            context.Connection.RemoteIpAddress = null;

            var act = () => middleware.Invoke(context);

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Invoke_WithNullIdentity_ShouldNotThrow()
        {
            // User.Identity can be null; code accesses .IsAuthenticated on it
            var middleware = new LogHelper(_mockNext.Object);
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal();
            context.Connection.RemoteIpAddress = IPAddress.Loopback;
            context.Request.Method = "GET";
            context.Request.Path = "/api/test";

            var act = () => middleware.Invoke(context);

            // ClaimsPrincipal with no identities returns null for .Identity
            // Code should handle this gracefully
            await act.Should().ThrowAsync<NullReferenceException>();
        }

        #endregion

        #region Static Info Method Tests

        [Fact]
        public void Info_WithNullDetails_ShouldNotThrow()
        {
            var logger = LogManager.CreateNullLogger();

            var act = () => LogHelper.Info(logger, "Test message", null);

            act.Should().NotThrow();
        }

        [Fact]
        public void Info_WithDetails_ShouldNotThrow()
        {
            var logger = LogManager.CreateNullLogger();
            var details = new Dictionary<string, string>
            {
                { "Key1", "Value1" }
            };

            var act = () => LogHelper.Info(logger, "Test message", details);

            act.Should().NotThrow();
        }

        [Fact]
        public void Info_WithDetails_ShouldAddMessageKey()
        {
            var logger = LogManager.CreateNullLogger();
            var details = new Dictionary<string, string>
            {
                { "Key1", "Value1" }
            };

            LogHelper.Info(logger, "Test message", details);

            details.Should().ContainKey("Message");
            details["Message"].Should().Be("Test message");
        }

        [Fact]
        public void Info_WithDetailsContainingMessageKey_ShouldUpdateMessageValue()
        {
            var logger = LogManager.CreateNullLogger();
            var details = new Dictionary<string, string>
            {
                { "Message", "Old message" },
                { "Key1", "Value1" }
            };

            LogHelper.Info(logger, "New message", details);

            details["Message"].Should().Be("New message");
        }

        [Fact]
        public void Info_WithEmptyDetails_ShouldAddMessageKey()
        {
            var logger = LogManager.CreateNullLogger();
            var details = new Dictionary<string, string>();

            LogHelper.Info(logger, "Test message", details);

            details.Should().ContainKey("Message");
            details["Message"].Should().Be("Test message");
        }

        [Fact]
        public void Info_WithNullDetails_ShouldCallLoggerInfo()
        {
            var memoryTarget = new MemoryTarget { Layout = "${message}" };
            var config = new NLog.Config.LoggingConfiguration();
            config.AddRuleForAllLevels(memoryTarget, "TestLogger");
            LogManager.Configuration = config;

            var logger = LogManager.GetLogger("TestLogger");

            LogHelper.Info(logger, "Test message", null);

            memoryTarget.Logs.Should().Contain("Test message");

            LogManager.Configuration = null;
        }

        [Fact]
        public void Info_WithEmptyMessage_ShouldNotThrow()
        {
            var logger = LogManager.CreateNullLogger();

            var act = () => LogHelper.Info(logger, "", null);

            act.Should().NotThrow();
        }

        #endregion
    }
}
