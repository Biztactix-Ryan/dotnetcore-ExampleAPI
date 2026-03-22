using System.Security.Claims;
using ExampleAPI.Contracts.Shared;
using ExampleAPI.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Targets;
using Xunit;

namespace ExampleAPI.Tests.Helpers
{
    [Collection("NLog")]
    public class JWTHelperTests
    {
        private static IConfiguration CreateConfiguration(string tokenPassword = "WhyHaveAStaticToken?")
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "BackendAuth:TokenPassword", tokenPassword }
                })
                .Build();
        }

        private static JWTHelper CreateHelper(RequestDelegate next = null)
        {
            return new JWTHelper(next ?? (_ => Task.CompletedTask));
        }

        private static HttpContext CreateAuthenticatedContext(IEnumerable<Claim> claims)
        {
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            var context = new DefaultHttpContext { User = principal };
            context.Connection.RemoteIpAddress = System.Net.IPAddress.Loopback;
            return context;
        }

        private static HttpContext CreateUnauthenticatedContext()
        {
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = System.Net.IPAddress.Loopback;
            return context;
        }

        // --- User extraction tests ---

        [Fact]
        public async Task Invoke_WithAuthenticatedUser_ShouldAttachLoggedinUser()
        {
            var userId = Guid.NewGuid();
            var claims = new[]
            {
                new Claim("Username", "testuser"),
                new Claim("Email", "test@example.com"),
                new Claim("UserID", userId.ToString())
            };
            var context = CreateAuthenticatedContext(claims);
            var helper = CreateHelper();

            await helper.Invoke(context, CreateConfiguration());

            context.Items.Should().ContainKey("User");
            var user = context.Items["User"] as LoggedinUser;
            user.Should().NotBeNull();
            user!.Username.Should().Be("testuser");
            user.Email.Should().Be("test@example.com");
            user.UserID.Should().Be(userId);
        }

        [Fact]
        public async Task Invoke_WithInvalidUserID_ShouldNotAttachUser()
        {
            var claims = new[]
            {
                new Claim("Username", "testuser"),
                new Claim("Email", "test@example.com"),
                new Claim("UserID", "not-a-guid")
            };
            var context = CreateAuthenticatedContext(claims);
            var helper = CreateHelper();

            await helper.Invoke(context, CreateConfiguration());

            context.Items.Should().NotContainKey("User");
        }

        [Fact]
        public async Task Invoke_WithMissingUserIDClaim_ShouldNotAttachUser()
        {
            var claims = new[]
            {
                new Claim("Username", "testuser"),
                new Claim("Email", "test@example.com")
            };
            var context = CreateAuthenticatedContext(claims);
            var helper = CreateHelper();

            await helper.Invoke(context, CreateConfiguration());

            context.Items.Should().NotContainKey("User");
        }

        [Fact]
        public async Task Invoke_WithUnauthenticatedUser_ShouldNotAttachUser()
        {
            var context = CreateUnauthenticatedContext();
            var helper = CreateHelper();

            await helper.Invoke(context, CreateConfiguration());

            context.Items.Should().NotContainKey("User");
        }

        [Fact]
        public async Task Invoke_ShouldCallNextMiddleware()
        {
            var nextCalled = false;
            var helper = CreateHelper(_ => { nextCalled = true; return Task.CompletedTask; });
            var context = CreateUnauthenticatedContext();

            await helper.Invoke(context, CreateConfiguration());

            nextCalled.Should().BeTrue();
        }

        [Fact]
        public async Task Invoke_WithAuthenticatedUser_ShouldCallNextMiddleware()
        {
            var nextCalled = false;
            var helper = CreateHelper(_ => { nextCalled = true; return Task.CompletedTask; });
            var userId = Guid.NewGuid();
            var claims = new[]
            {
                new Claim("Username", "testuser"),
                new Claim("Email", "test@example.com"),
                new Claim("UserID", userId.ToString())
            };
            var context = CreateAuthenticatedContext(claims);

            await helper.Invoke(context, CreateConfiguration());

            nextCalled.Should().BeTrue();
        }

        // --- Backend token verification tests ---

        [Fact]
        public async Task Invoke_WithoutBackendClaim_ShouldSetBackendAuthFalse()
        {
            var userId = Guid.NewGuid();
            var claims = new[]
            {
                new Claim("Username", "testuser"),
                new Claim("Email", "test@example.com"),
                new Claim("UserID", userId.ToString())
            };
            var context = CreateAuthenticatedContext(claims);
            var helper = CreateHelper();

            await helper.Invoke(context, CreateConfiguration());

            context.Items["BackendAuth"].Should().Be(false);
        }

        [Fact]
        public async Task Invoke_WithValidBackendToken_ShouldSetBackendAuthTrue()
        {
            var userId = Guid.NewGuid();
            var salt = new byte[16];
            new Random(42).NextBytes(salt);
            var saltBase64 = Convert.ToBase64String(salt);

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: "WhyHaveAStaticToken?",
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            var claims = new[]
            {
                new Claim("Username", "backenduser"),
                new Claim("Email", "backend@test.com"),
                new Claim("UserID", userId.ToString()),
                new Claim("BACKEND", "TRUE"),
                new Claim("Token", saltBase64),
                new Claim("Check", hashed),
                new Claim("AppName", "TestApp")
            };
            var context = CreateAuthenticatedContext(claims);
            var helper = CreateHelper();

            await helper.Invoke(context, CreateConfiguration());

            context.Items["BackendAuth"].Should().Be(true);
            context.Items["BackendApp"].Should().Be("TestApp");
        }

        [Fact]
        public async Task Invoke_WithInvalidBackendCheck_ShouldSetBackendAuthFalse()
        {
            var userId = Guid.NewGuid();
            var salt = new byte[16];
            new Random(42).NextBytes(salt);
            var saltBase64 = Convert.ToBase64String(salt);

            var claims = new[]
            {
                new Claim("Username", "backenduser"),
                new Claim("Email", "backend@test.com"),
                new Claim("UserID", userId.ToString()),
                new Claim("BACKEND", "TRUE"),
                new Claim("Token", saltBase64),
                new Claim("Check", "InvalidHashValue"),
                new Claim("AppName", "TestApp")
            };
            var context = CreateAuthenticatedContext(claims);
            var helper = CreateHelper();

            await helper.Invoke(context, CreateConfiguration());

            context.Items["BackendAuth"].Should().Be(false);
        }

        [Fact]
        public async Task Invoke_WithBackendFalse_ShouldNotVerifyBackendToken()
        {
            var userId = Guid.NewGuid();
            var claims = new[]
            {
                new Claim("Username", "testuser"),
                new Claim("Email", "test@example.com"),
                new Claim("UserID", userId.ToString()),
                new Claim("BACKEND", "FALSE")
            };
            var context = CreateAuthenticatedContext(claims);
            var helper = CreateHelper();

            await helper.Invoke(context, CreateConfiguration());

            context.Items["BackendAuth"].Should().Be(false);
            context.Items.Should().NotContainKey("BackendApp");
        }

        // --- Null-conditional / null-check verification tests (US-EX-21-1) ---

        [Fact]
        public async Task Invoke_WithBackendTrue_MissingTokenClaim_ShouldNotThrow()
        {
            var userId = Guid.NewGuid();
            var claims = new[]
            {
                new Claim("Username", "backenduser"),
                new Claim("Email", "backend@test.com"),
                new Claim("UserID", userId.ToString()),
                new Claim("BACKEND", "TRUE"),
                new Claim("Check", "somehash"),
                new Claim("AppName", "TestApp")
            };
            var context = CreateAuthenticatedContext(claims);
            var helper = CreateHelper();

            var act = () => helper.Invoke(context, CreateConfiguration());

            await act.Should().NotThrowAsync();
            context.Items["BackendAuth"].Should().Be(false);
        }

        [Fact]
        public async Task Invoke_WithBackendTrue_MissingCheckClaim_ShouldNotThrow()
        {
            var userId = Guid.NewGuid();
            var salt = new byte[16];
            new Random(42).NextBytes(salt);
            var claims = new[]
            {
                new Claim("Username", "backenduser"),
                new Claim("Email", "backend@test.com"),
                new Claim("UserID", userId.ToString()),
                new Claim("BACKEND", "TRUE"),
                new Claim("Token", Convert.ToBase64String(salt)),
                new Claim("AppName", "TestApp")
            };
            var context = CreateAuthenticatedContext(claims);
            var helper = CreateHelper();

            var act = () => helper.Invoke(context, CreateConfiguration());

            await act.Should().NotThrowAsync();
            context.Items["BackendAuth"].Should().Be(false);
        }

        [Fact]
        public async Task Invoke_WithBackendTrue_MissingAppNameClaim_ShouldNotThrow()
        {
            var userId = Guid.NewGuid();
            var salt = new byte[16];
            new Random(42).NextBytes(salt);
            var claims = new[]
            {
                new Claim("Username", "backenduser"),
                new Claim("Email", "backend@test.com"),
                new Claim("UserID", userId.ToString()),
                new Claim("BACKEND", "TRUE"),
                new Claim("Token", Convert.ToBase64String(salt)),
                new Claim("Check", "somehash")
            };
            var context = CreateAuthenticatedContext(claims);
            var helper = CreateHelper();

            var act = () => helper.Invoke(context, CreateConfiguration());

            await act.Should().NotThrowAsync();
            context.Items["BackendAuth"].Should().Be(false);
        }

        [Fact]
        public async Task Invoke_WithBackendTrue_AllBackendClaimsMissing_ShouldNotThrow()
        {
            var userId = Guid.NewGuid();
            var claims = new[]
            {
                new Claim("Username", "backenduser"),
                new Claim("Email", "backend@test.com"),
                new Claim("UserID", userId.ToString()),
                new Claim("BACKEND", "TRUE")
            };
            var context = CreateAuthenticatedContext(claims);
            var helper = CreateHelper();

            var act = () => helper.Invoke(context, CreateConfiguration());

            await act.Should().NotThrowAsync();
            context.Items["BackendAuth"].Should().Be(false);
        }

        [Fact]
        public async Task Invoke_WithAllClaimsMissing_ShouldNotThrow()
        {
            var context = CreateAuthenticatedContext(Array.Empty<Claim>());
            var helper = CreateHelper();

            var act = () => helper.Invoke(context, CreateConfiguration());

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Invoke_MissingUsernameClaim_ShouldNotThrow()
        {
            var userId = Guid.NewGuid();
            var claims = new[]
            {
                new Claim("Email", "test@example.com"),
                new Claim("UserID", userId.ToString())
            };
            var context = CreateAuthenticatedContext(claims);
            var helper = CreateHelper();

            var act = () => helper.Invoke(context, CreateConfiguration());

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Invoke_MissingEmailClaim_ShouldNotThrow()
        {
            var userId = Guid.NewGuid();
            var claims = new[]
            {
                new Claim("Username", "testuser"),
                new Claim("UserID", userId.ToString())
            };
            var context = CreateAuthenticatedContext(claims);
            var helper = CreateHelper();

            var act = () => helper.Invoke(context, CreateConfiguration());

            await act.Should().NotThrowAsync();
        }

        // --- Configuration-based token password tests (US-EX-22-1) ---

        [Fact]
        public async Task Invoke_BackendToken_ShouldUsePasswordFromConfiguration()
        {
            var customPassword = "CustomConfiguredPassword123!";
            var userId = Guid.NewGuid();
            var salt = new byte[16];
            new Random(42).NextBytes(salt);
            var saltBase64 = Convert.ToBase64String(salt);

            // Hash using the custom password (simulating what a caller would do)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: customPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            var claims = new[]
            {
                new Claim("Username", "backenduser"),
                new Claim("Email", "backend@test.com"),
                new Claim("UserID", userId.ToString()),
                new Claim("BACKEND", "TRUE"),
                new Claim("Token", saltBase64),
                new Claim("Check", hashed),
                new Claim("AppName", "TestApp")
            };
            var context = CreateAuthenticatedContext(claims);
            var helper = CreateHelper();

            await helper.Invoke(context, CreateConfiguration(customPassword));

            context.Items["BackendAuth"].Should().Be(true);
            context.Items["BackendApp"].Should().Be("TestApp");
        }

        [Fact]
        public async Task Invoke_BackendToken_WithWrongConfigPassword_ShouldRejectToken()
        {
            var userId = Guid.NewGuid();
            var salt = new byte[16];
            new Random(42).NextBytes(salt);
            var saltBase64 = Convert.ToBase64String(salt);

            // Hash with one password
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: "CorrectPassword",
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            var claims = new[]
            {
                new Claim("Username", "backenduser"),
                new Claim("Email", "backend@test.com"),
                new Claim("UserID", userId.ToString()),
                new Claim("BACKEND", "TRUE"),
                new Claim("Token", saltBase64),
                new Claim("Check", hashed),
                new Claim("AppName", "TestApp")
            };
            var context = CreateAuthenticatedContext(claims);
            var helper = CreateHelper();

            // Verify with a different password from config — should fail
            await helper.Invoke(context, CreateConfiguration("WrongPassword"));

            context.Items["BackendAuth"].Should().Be(false);
        }

        [Fact]
        public async Task Invoke_BackendToken_WithMissingConfigPassword_ShouldThrow()
        {
            var userId = Guid.NewGuid();
            var salt = new byte[16];
            new Random(42).NextBytes(salt);

            var claims = new[]
            {
                new Claim("Username", "backenduser"),
                new Claim("Email", "backend@test.com"),
                new Claim("UserID", userId.ToString()),
                new Claim("BACKEND", "TRUE"),
                new Claim("Token", Convert.ToBase64String(salt)),
                new Claim("Check", "somehash"),
                new Claim("AppName", "TestApp")
            };
            var context = CreateAuthenticatedContext(claims);
            var helper = CreateHelper();

            // Empty configuration — no BackendAuth:TokenPassword
            var emptyConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>())
                .Build();

            var act = () => helper.Invoke(context, emptyConfig);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*BackendAuth:TokenPassword*");
        }

        // --- Null RemoteIpAddress tests (US-EX-28-1) ---

        [Fact]
        public async Task Invoke_AuthenticatedUser_NullRemoteIpAddress_ShouldNotThrow()
        {
            var userId = Guid.NewGuid();
            var claims = new[]
            {
                new Claim("Username", "testuser"),
                new Claim("Email", "test@example.com"),
                new Claim("UserID", userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            var context = new DefaultHttpContext { User = principal };
            // RemoteIpAddress is null by default — do not set it

            var helper = CreateHelper();

            var act = () => helper.Invoke(context, CreateConfiguration());

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Invoke_UnauthenticatedUser_NullRemoteIpAddress_ShouldNotThrow()
        {
            var context = new DefaultHttpContext();
            // RemoteIpAddress is null by default — do not set it

            var helper = CreateHelper();

            var act = () => helper.Invoke(context, CreateConfiguration());

            await act.Should().NotThrowAsync();
        }

        // --- Fallback "unknown" value tests (US-EX-28-2) ---

        [Fact]
        public async Task Invoke_AuthenticatedUser_NullRemoteIpAddress_ShouldLogUnknownFallback()
        {
            var memoryTarget = new MemoryTarget { Layout = "${message}" };
            var config = new NLog.Config.LoggingConfiguration();
            config.AddRuleForAllLevels(memoryTarget);
            LogManager.Configuration = config;
            LogManager.ReconfigExistingLoggers();

            var userId = Guid.NewGuid();
            var claims = new[]
            {
                new Claim("Username", "testuser"),
                new Claim("Email", "test@example.com"),
                new Claim("UserID", userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            var context = new DefaultHttpContext { User = principal };
            // RemoteIpAddress is null by default

            var helper = CreateHelper();
            await helper.Invoke(context, CreateConfiguration());

            memoryTarget.Logs.Should().Contain(log => log.Contains("unknown"),
                "when RemoteIpAddress is null, the fallback value 'unknown' should appear in the log");
        }

        [Fact]
        public async Task Invoke_UnauthenticatedUser_NullRemoteIpAddress_ShouldLogUnknownFallback()
        {
            var memoryTarget = new MemoryTarget { Layout = "${message}" };
            var config = new NLog.Config.LoggingConfiguration();
            config.AddRuleForAllLevels(memoryTarget);
            LogManager.Configuration = config;
            LogManager.ReconfigExistingLoggers();

            var context = new DefaultHttpContext();
            // RemoteIpAddress is null by default

            var helper = CreateHelper();
            await helper.Invoke(context, CreateConfiguration());

            memoryTarget.Logs.Should().Contain(log => log.Contains("unknown"),
                "when RemoteIpAddress is null, the fallback value 'unknown' should appear in the log");
        }

        [Fact]
        public async Task Invoke_AuthenticatedUser_WithRemoteIpAddress_ShouldNotLogUnknownForIp()
        {
            var memoryTarget = new MemoryTarget { Layout = "${message}" };
            var config = new NLog.Config.LoggingConfiguration();
            config.AddRuleForAllLevels(memoryTarget);
            LogManager.Configuration = config;
            LogManager.ReconfigExistingLoggers();

            var userId = Guid.NewGuid();
            var claims = new[]
            {
                new Claim("Username", "testuser"),
                new Claim("Email", "test@example.com"),
                new Claim("UserID", userId.ToString())
            };
            var context = CreateAuthenticatedContext(claims);
            var helper = CreateHelper();

            await helper.Invoke(context, CreateConfiguration());

            memoryTarget.Logs.Should().Contain(log => log.Contains("127.0.0.1"),
                "when RemoteIpAddress is set, the actual IP should appear in the log instead of 'unknown'");
        }

        // --- No NullReferenceException tests (US-EX-28-3) ---

        [Fact]
        public async Task Invoke_AuthenticatedUser_NullRemoteIpAddress_ShouldNotThrowNullReferenceException()
        {
            var userId = Guid.NewGuid();
            var claims = new[]
            {
                new Claim("Username", "testuser"),
                new Claim("Email", "test@example.com"),
                new Claim("UserID", userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            var context = new DefaultHttpContext { User = principal };
            // RemoteIpAddress is null by default — do not set it

            var helper = CreateHelper();

            var act = () => helper.Invoke(context, CreateConfiguration());

            await act.Should().NotThrowAsync<NullReferenceException>();
        }

        [Fact]
        public async Task Invoke_UnauthenticatedUser_NullRemoteIpAddress_ShouldNotThrowNullReferenceException()
        {
            var context = new DefaultHttpContext();
            // RemoteIpAddress is null by default — do not set it

            var helper = CreateHelper();

            var act = () => helper.Invoke(context, CreateConfiguration());

            await act.Should().NotThrowAsync<NullReferenceException>();
        }

        [Fact]
        public void Appsettings_ShouldContainBackendAuthTokenPassword()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var tokenPassword = config["BackendAuth:TokenPassword"];
            tokenPassword.Should().NotBeNullOrEmpty(
                "appsettings.json must contain BackendAuth:TokenPassword for backend auth to work");
        }
    }
}
