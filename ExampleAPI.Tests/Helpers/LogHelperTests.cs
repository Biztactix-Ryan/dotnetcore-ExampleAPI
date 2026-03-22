using System.Net;
using System.Security.Claims;
using ExampleAPI.Contracts.Shared;
using ExampleAPI.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NLog;
using NLog.Targets;
using Xunit;

namespace ExampleAPI.Tests.Helpers
{
    [Collection("NLog")]
    public class LogHelperTests : IDisposable
    {
        private readonly MemoryTarget _memoryTarget;

        public LogHelperTests()
        {
            _memoryTarget = new MemoryTarget { Layout = "${message}|${all-event-properties}" };
            var config = new NLog.Config.LoggingConfiguration();
            config.AddRuleForAllLevels(_memoryTarget);
            LogManager.Configuration = config;
            LogManager.ReconfigExistingLoggers();
        }

        public void Dispose()
        {
            LogManager.Configuration = null;
        }

        private static LogHelper CreateHelper(RequestDelegate next = null)
        {
            return new LogHelper(next ?? (_ => Task.CompletedTask));
        }

        private static DefaultHttpContext CreateAuthenticatedContext(Guid? userId = null)
        {
            var uid = userId ?? Guid.NewGuid();
            var claims = new[]
            {
                new Claim("Username", "testuser"),
                new Claim("Email", "test@example.com"),
                new Claim("UserID", uid.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            var context = new DefaultHttpContext { User = principal };
            context.Connection.RemoteIpAddress = IPAddress.Loopback;
            context.Items["User"] = new LoggedinUser
            {
                Username = "testuser",
                Email = "test@example.com",
                UserID = uid
            };
            return context;
        }

        // --- Constructor tests ---

        [Fact]
        public void Constructor_ShouldAcceptRequestDelegate()
        {
            RequestDelegate next = _ => Task.CompletedTask;

            var helper = new LogHelper(next);

            helper.Should().NotBeNull();
        }

        // --- Invoke: basic delegate execution ---

        [Fact]
        public async Task Invoke_ShouldCallNextDelegate()
        {
            var wasCalled = false;
            RequestDelegate next = _ => { wasCalled = true; return Task.CompletedTask; };
            var helper = new LogHelper(next);
            var context = new DefaultHttpContext();

            await helper.Invoke(context);

            wasCalled.Should().BeTrue();
        }

        // --- Invoke: authenticated user path ---

        [Fact]
        public async Task Invoke_AuthenticatedUser_ShouldLogUserID()
        {
            var userId = Guid.NewGuid();
            var context = CreateAuthenticatedContext(userId);
            var helper = CreateHelper();

            await helper.Invoke(context);

            _memoryTarget.Logs.Should().Contain(log => log.Contains(userId.ToString()));
        }

        [Fact]
        public async Task Invoke_AuthenticatedUser_ShouldLogApiName()
        {
            var context = CreateAuthenticatedContext();
            var helper = CreateHelper();

            await helper.Invoke(context);

            _memoryTarget.Logs.Should().Contain(log => log.Contains("ExampleAPI"));
        }

        [Fact]
        public async Task Invoke_AuthenticatedUser_ShouldLogIP()
        {
            var context = CreateAuthenticatedContext();
            context.Connection.RemoteIpAddress = IPAddress.Parse("192.168.1.1");
            var helper = CreateHelper();

            await helper.Invoke(context);

            _memoryTarget.Logs.Should().Contain(log => log.Contains("192.168.1.1"));
        }

        [Fact]
        public async Task Invoke_AuthenticatedUser_ShouldLogResponseStatusCode()
        {
            var context = CreateAuthenticatedContext();
            context.Response.StatusCode = 201;
            var helper = CreateHelper();

            await helper.Invoke(context);

            _memoryTarget.Logs.Should().Contain(log => log.Contains("201"));
        }

        // --- Invoke: unauthenticated user path ---

        [Fact]
        public async Task Invoke_UnauthenticatedUser_ShouldLogWithoutUserID()
        {
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = IPAddress.Loopback;
            var helper = CreateHelper();

            await helper.Invoke(context);

            _memoryTarget.Logs.Should().HaveCountGreaterThan(0);
            // Should not contain a "User" property value (no user ID logged)
            _memoryTarget.Logs.Should().NotContain(log => log.Contains("User="));
        }

        [Fact]
        public async Task Invoke_UnauthenticatedUser_ShouldLogMethod()
        {
            var context = new DefaultHttpContext();
            context.Request.Method = "POST";
            context.Connection.RemoteIpAddress = IPAddress.Loopback;
            var helper = CreateHelper();

            await helper.Invoke(context);

            _memoryTarget.Logs.Should().Contain(log => log.Contains("POST"));
        }

        // --- Invoke: null RemoteIpAddress fallback ---

        [Fact]
        public async Task Invoke_AuthenticatedUser_NullRemoteIpAddress_ShouldNotThrow()
        {
            var context = CreateAuthenticatedContext();
            // Override to null - DefaultHttpContext leaves it null by default but our helper sets it
            context.Connection.RemoteIpAddress = null;
            var helper = CreateHelper();

            var act = () => helper.Invoke(context);

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Invoke_UnauthenticatedUser_NullRemoteIpAddress_ShouldNotThrow()
        {
            var context = new DefaultHttpContext();
            var helper = CreateHelper();

            var act = () => helper.Invoke(context);

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Invoke_NullRemoteIpAddress_ShouldLogUnknownFallback()
        {
            var context = new DefaultHttpContext();
            var helper = CreateHelper();

            await helper.Invoke(context);

            _memoryTarget.Logs.Should().Contain(log => log.Contains("unknown"));
        }

        [Fact]
        public async Task Invoke_WithRemoteIpAddress_ShouldLogActualIp()
        {
            var context = CreateAuthenticatedContext();
            context.Connection.RemoteIpAddress = IPAddress.Loopback;
            var helper = CreateHelper();

            await helper.Invoke(context);

            _memoryTarget.Logs.Should().Contain(log => log.Contains("127.0.0.1"));
        }

        // --- Invoke: exception propagation ---

        [Fact]
        public async Task Invoke_WhenNextThrows_ShouldPropagateException()
        {
            RequestDelegate next = _ => throw new InvalidOperationException("test error");
            var helper = new LogHelper(next);
            var context = new DefaultHttpContext();

            var act = () => helper.Invoke(context);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("test error");
        }

        // --- Invoke: HTTP method and path logging ---

        [Theory]
        [InlineData("GET", "/api/v1/example")]
        [InlineData("POST", "/api/v1/auth/login")]
        [InlineData("DELETE", "/api/v1/items/123")]
        public async Task Invoke_ShouldLogMethodAndPath(string method, string path)
        {
            var context = new DefaultHttpContext();
            context.Request.Method = method;
            context.Request.Path = path;
            context.Connection.RemoteIpAddress = IPAddress.Loopback;
            var helper = CreateHelper();

            await helper.Invoke(context);

            _memoryTarget.Logs.Should().Contain(log => log.Contains(method));
            _memoryTarget.Logs.Should().Contain(log => log.Contains(path));
        }

        // --- Invoke: authenticated but no LoggedinUser in Items ---

        [Fact]
        public async Task Invoke_AuthenticatedButNoLoggedinUserInItems_ShouldTakeUnauthPath()
        {
            var identity = new ClaimsIdentity(new[] { new Claim("Username", "test") }, "TestAuth");
            var context = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
            context.Connection.RemoteIpAddress = IPAddress.Loopback;
            // Don't set context.Items["User"]
            var helper = CreateHelper();

            var act = () => helper.Invoke(context);

            await act.Should().NotThrowAsync();
        }

        // --- Static Info method tests ---

        [Fact]
        public void Info_WithNullDetails_ShouldLogInfoLevel()
        {
            var logger = LogManager.GetCurrentClassLogger();

            var act = () => LogHelper.Info(logger, "Test message", null);

            act.Should().NotThrow();
            _memoryTarget.Logs.Should().Contain(log => log.Contains("Test message"));
        }

        [Fact]
        public void Info_WithDetails_ShouldAddMessageKeyToDetails()
        {
            var logger = LogManager.GetCurrentClassLogger();
            var details = new Dictionary<string, string> { { "Key1", "Value1" } };

            LogHelper.Info(logger, "Test message", details);

            details.Should().ContainKey("Message")
                .WhoseValue.Should().Be("Test message");
        }

        [Fact]
        public void Info_WithDetails_ExistingMessageKey_ShouldOverwriteMessageValue()
        {
            var logger = LogManager.GetCurrentClassLogger();
            var details = new Dictionary<string, string>
            {
                { "Message", "old message" },
                { "Key1", "Value1" }
            };

            LogHelper.Info(logger, "New message", details);

            details["Message"].Should().Be("New message");
        }

        [Fact]
        public void Info_WithEmptyDetails_ShouldAddMessageKey()
        {
            var logger = LogManager.GetCurrentClassLogger();
            var details = new Dictionary<string, string>();

            LogHelper.Info(logger, "Test message", details);

            details.Should().ContainKey("Message")
                .WhoseValue.Should().Be("Test message");
        }
    }
}
