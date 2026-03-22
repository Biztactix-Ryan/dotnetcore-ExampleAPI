using ExampleAPI.Installers;
using ExampleAPI.Options;
using ExampleAPI.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace ExampleAPI.Tests.Installers
{
    public class JWTInstallerTests
    {
        private readonly ServiceCollection _services;
        private readonly IConfiguration _configuration;

        public JWTInstallerTests()
        {
            _services = new ServiceCollection();
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "JwtSettings:Secret", "TestSecretKeyThatIsLongEnoughForHmacSha256!!" }
                })
                .Build();
        }

        [Fact]
        public void InstallServices_RegistersJwtSettings()
        {
            var installer = new JWTInstaller();

            installer.InstallServices(_services, _configuration);

            _services.Should().Contain(sd => sd.ServiceType == typeof(JwtSettings));
        }

        [Fact]
        public void InstallServices_RegistersTokenValidationParameters()
        {
            var installer = new JWTInstaller();

            installer.InstallServices(_services, _configuration);

            _services.Should().Contain(sd =>
                sd.ServiceType == typeof(TokenValidationParameters));
        }

        [Fact]
        public void InstallServices_RegistersJWTService()
        {
            var installer = new JWTInstaller();

            installer.InstallServices(_services, _configuration);

            _services.Should().Contain(sd => sd.ServiceType == typeof(JWTService));
        }

        [Fact]
        public void InstallServices_JWTServiceResolvableFromContainer()
        {
            var installer = new JWTInstaller();

            installer.InstallServices(_services, _configuration);

            var provider = _services.BuildServiceProvider();
            var jwtService = provider.GetRequiredService<JWTService>();
            jwtService.Should().NotBeNull();
        }

        [Fact]
        public void InstallServices_BindsConfigFromJwtSettingsSection()
        {
            var expectedSecret = "TestSecretKeyThatIsLongEnoughForHmacSha256!!";
            var installer = new JWTInstaller();

            installer.InstallServices(_services, _configuration);

            var provider = _services.BuildServiceProvider();
            var jwtSettings = provider.GetRequiredService<JwtSettings>();
            jwtSettings.Secret.Should().Be(expectedSecret);
        }

        [Fact]
        public void InstallServices_FailsWhenSectionMissing()
        {
            var services = new ServiceCollection();
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "WrongSection:Secret", "WrongSectionSecret" }
                })
                .Build();
            var installer = new JWTInstaller();

            var act = () => installer.InstallServices(services, config);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void InstallServices_SetsValidateIssuerToTrue()
        {
            var installer = new JWTInstaller();

            installer.InstallServices(_services, _configuration);

            var provider = _services.BuildServiceProvider();
            var tokenParams = provider.GetRequiredService<TokenValidationParameters>();
            tokenParams.ValidateIssuer.Should().BeTrue();
        }

        [Fact]
        public void InstallServices_SetsValidateAudienceToTrue()
        {
            var installer = new JWTInstaller();

            installer.InstallServices(_services, _configuration);

            var provider = _services.BuildServiceProvider();
            var tokenParams = provider.GetRequiredService<TokenValidationParameters>();
            tokenParams.ValidateAudience.Should().BeTrue();
        }

        [Fact]
        public void InstallServices_SetsRequireExpirationTimeToTrue()
        {
            var installer = new JWTInstaller();

            installer.InstallServices(_services, _configuration);

            var provider = _services.BuildServiceProvider();
            var tokenParams = provider.GetRequiredService<TokenValidationParameters>();
            tokenParams.RequireExpirationTime.Should().BeTrue();
        }

        [Fact]
        public void InstallServices_SetsValidIssuerFromSettings()
        {
            var services = new ServiceCollection();
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "JwtSettings:Secret", "TestSecretKeyThatIsLongEnoughForHmacSha256!!" },
                    { "JwtSettings:Issuer", "MyTestIssuer" }
                })
                .Build();
            var installer = new JWTInstaller();

            installer.InstallServices(services, config);

            var provider = services.BuildServiceProvider();
            var tokenParams = provider.GetRequiredService<TokenValidationParameters>();
            tokenParams.ValidIssuer.Should().Be("MyTestIssuer");
        }

        [Fact]
        public void InstallServices_SetsValidAudienceFromSettings()
        {
            var services = new ServiceCollection();
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "JwtSettings:Secret", "TestSecretKeyThatIsLongEnoughForHmacSha256!!" },
                    { "JwtSettings:Audience", "MyTestAudience" }
                })
                .Build();
            var installer = new JWTInstaller();

            installer.InstallServices(services, config);

            var provider = services.BuildServiceProvider();
            var tokenParams = provider.GetRequiredService<TokenValidationParameters>();
            tokenParams.ValidAudience.Should().Be("MyTestAudience");
        }

        [Fact]
        public void InstallServices_RegistersAuthentication()
        {
            var installer = new JWTInstaller();

            installer.InstallServices(_services, _configuration);

            _services.Should().Contain(sd =>
                sd.ServiceType.FullName.Contains("Authentication"));
        }
    }
}
