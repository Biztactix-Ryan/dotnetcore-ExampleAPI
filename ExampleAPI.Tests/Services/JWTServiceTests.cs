using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ExampleAPI.Contracts.Shared;
using ExampleAPI.Options;
using ExampleAPI.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace ExampleAPI.Tests.Services
{
    public class JWTServiceTests
    {
        private readonly JwtSettings _settings;
        private readonly TokenValidationParameters _tokenParams;
        private readonly JWTService _service;

        public JWTServiceTests()
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
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true
            };

            _service = new JWTService(_settings, _tokenParams);
        }

        [Fact]
        public void GenerateJSONWebToken_ShouldReturnValidToken()
        {
            var user = new LoggedinUser
            {
                Username = "testuser",
                Email = "test@example.com",
                UserID = Guid.NewGuid()
            };

            var token = _service.GenerateJSONWebToken(user);

            token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void GenerateJSONWebToken_ShouldContainUsernameClaim()
        {
            var user = new LoggedinUser
            {
                Username = "testuser",
                Email = "test@example.com",
                UserID = Guid.NewGuid()
            };

            var token = _service.GenerateJSONWebToken(user);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            jwt.Claims.Should().Contain(c => c.Type == "Username" && c.Value == "testuser");
        }

        [Fact]
        public void GenerateJSONWebToken_ShouldContainEmailClaim()
        {
            var user = new LoggedinUser
            {
                Username = "testuser",
                Email = "test@example.com",
                UserID = Guid.NewGuid()
            };

            var token = _service.GenerateJSONWebToken(user);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            jwt.Claims.Should().Contain(c => c.Type == "Email" && c.Value == "test@example.com");
        }

        [Fact]
        public void GenerateJSONWebToken_ShouldContainJtiClaim()
        {
            var user = new LoggedinUser
            {
                Username = "testuser",
                Email = "test@example.com",
                UserID = Guid.NewGuid()
            };

            var token = _service.GenerateJSONWebToken(user);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Jti);
        }

        [Fact]
        public void GenerateJSONWebToken_ShouldContainUserIDClaim()
        {
            var userId = Guid.NewGuid();
            var user = new LoggedinUser
            {
                Username = "testuser",
                Email = "test@example.com",
                UserID = userId
            };

            var token = _service.GenerateJSONWebToken(user);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            jwt.Claims.Should().Contain(c => c.Type == "UserID" && c.Value == userId.ToString());
        }

        [Fact]
        public void GenerateJSONWebToken_ShouldSetCorrectIssuer()
        {
            var user = new LoggedinUser
            {
                Username = "testuser",
                Email = "test@example.com",
                UserID = Guid.NewGuid()
            };

            var token = _service.GenerateJSONWebToken(user);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            jwt.Issuer.Should().Be("TestIssuer");
        }

        [Fact]
        public void GenerateJSONWebToken_ShouldSetExpiration()
        {
            var user = new LoggedinUser
            {
                Username = "testuser",
                Email = "test@example.com",
                UserID = Guid.NewGuid()
            };

            var beforeGeneration = DateTime.UtcNow;
            var token = _service.GenerateJSONWebToken(user);
            var afterGeneration = DateTime.UtcNow;

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            jwt.ValidTo.Should().BeAfter(beforeGeneration.AddMinutes(59));
            jwt.ValidTo.Should().BeBefore(afterGeneration.AddMinutes(61));
        }

        [Fact]
        public void GenerateJSONWebToken_EachCall_ShouldGenerateUniqueJti()
        {
            var user = new LoggedinUser
            {
                Username = "testuser",
                Email = "test@example.com",
                UserID = Guid.NewGuid()
            };

            var token1 = _service.GenerateJSONWebToken(user);
            var token2 = _service.GenerateJSONWebToken(user);

            var handler = new JwtSecurityTokenHandler();
            var jti1 = handler.ReadJwtToken(token1).Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
            var jti2 = handler.ReadJwtToken(token2).Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

            jti1.Should().NotBe(jti2);
        }

        [Fact]
        public void GenerateJSONWebToken_ShouldBeValidatable()
        {
            var user = new LoggedinUser
            {
                Username = "testuser",
                Email = "test@example.com",
                UserID = Guid.NewGuid()
            };

            var token = _service.GenerateJSONWebToken(user);

            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, _tokenParams, out var validatedToken);

            principal.Should().NotBeNull();
            validatedToken.Should().NotBeNull();
        }

        [Fact]
        public void ValidateToken_WithWrongSigningKey_ShouldThrow()
        {
            var user = new LoggedinUser
            {
                Username = "testuser",
                Email = "test@example.com",
                UserID = Guid.NewGuid()
            };

            var token = _service.GenerateJSONWebToken(user);

            var wrongKeyParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("ThisIsACompletelyDifferentSecretKeyForTestingPurposes!!")),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            };

            var handler = new JwtSecurityTokenHandler();
            var act = () => handler.ValidateToken(token, wrongKeyParams, out _);

            act.Should().Throw<SecurityTokenSignatureKeyNotFoundException>();
        }

        [Fact]
        public void ValidateToken_WithTamperedToken_ShouldThrow()
        {
            var user = new LoggedinUser
            {
                Username = "testuser",
                Email = "test@example.com",
                UserID = Guid.NewGuid()
            };

            var token = _service.GenerateJSONWebToken(user);

            // Tamper with the payload by flipping a character
            var parts = token.Split('.');
            var tamperedPayload = parts[1][..^1] + (parts[1][^1] == 'A' ? 'B' : 'A');
            var tamperedToken = $"{parts[0]}.{tamperedPayload}.{parts[2]}";

            var handler = new JwtSecurityTokenHandler();
            var act = () => handler.ValidateToken(tamperedToken, _tokenParams, out _);

            act.Should().Throw<Exception>();
        }

        [Fact]
        public void ValidateToken_WithExpiredToken_ShouldThrow()
        {
            var user = new LoggedinUser
            {
                Username = "testuser",
                Email = "test@example.com",
                UserID = Guid.NewGuid()
            };

            // Manually create an already-expired token
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);
            var expiredToken = new JwtSecurityToken(
                _settings.Issuer, _settings.Issuer,
                new[] { new Claim("Username", user.Username) },
                expires: DateTime.UtcNow.AddMinutes(-5),
                signingCredentials: credentials);
            var token = new JwtSecurityTokenHandler().WriteToken(expiredToken);

            var handler = new JwtSecurityTokenHandler();
            var act = () => handler.ValidateToken(token, _tokenParams, out _);

            act.Should().Throw<SecurityTokenExpiredException>();
        }

        [Fact]
        public void ValidateToken_ShouldExtractCorrectClaims()
        {
            var userId = Guid.NewGuid();
            var user = new LoggedinUser
            {
                Username = "claimuser",
                Email = "claim@test.com",
                UserID = userId
            };

            var token = _service.GenerateJSONWebToken(user);

            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, _tokenParams, out _);

            principal.FindFirst("Username")!.Value.Should().Be("claimuser");
            principal.FindFirst("Email")!.Value.Should().Be("claim@test.com");
            principal.FindFirst("UserID")!.Value.Should().Be(userId.ToString());
        }

        [Fact]
        public void ValidateToken_ShouldUseHmacSha512Algorithm()
        {
            var user = new LoggedinUser
            {
                Username = "testuser",
                Email = "test@example.com",
                UserID = Guid.NewGuid()
            };

            var token = _service.GenerateJSONWebToken(user);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            jwt.Header.Alg.Should().Be("HS512");
        }

        [Fact]
        public void GenerateJSONWebToken_WithConfiguredSettings_ShouldProduceValidatableToken()
        {
            // Arrange: simulate settings loaded from configuration (as JWTInstaller does)
            var configSecret = "ConfiguredSecretKeyLongEnoughForHmacSha512Algorithm!!";
            var configIssuer = "ConfiguredIssuer";
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "JwtSettings:Secret", configSecret },
                    { "JwtSettings:Issuer", configIssuer },
                    { "JwtSettings:TokenLifetime", "02:00:00" }
                })
                .Build();

            var configuredSettings = new JwtSettings();
            configuration.Bind("JwtSettings", configuredSettings);

            var validationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuredSettings.Secret)),
                ValidateIssuer = true,
                ValidIssuer = configIssuer,
                ValidateAudience = false,
                ValidateLifetime = true
            };

            var service = new JWTService(configuredSettings, validationParams);
            var user = new LoggedinUser
            {
                Username = "configuser",
                Email = "config@example.com",
                UserID = Guid.NewGuid()
            };

            // Act
            var token = service.GenerateJSONWebToken(user);

            // Assert: token is valid and carries the configured issuer & lifetime
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, validationParams, out var validatedToken);
            principal.Should().NotBeNull();

            var jwt = handler.ReadJwtToken(token);
            jwt.Issuer.Should().Be(configIssuer);
            jwt.ValidTo.Should().BeCloseTo(
                DateTime.UtcNow.AddHours(2), precision: TimeSpan.FromMinutes(1));
            principal.FindFirst("Username")!.Value.Should().Be("configuser");
            principal.FindFirst("Email")!.Value.Should().Be("config@example.com");
        }
    }
}
