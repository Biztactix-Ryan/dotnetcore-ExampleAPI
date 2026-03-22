using ExampleAPI.Installers;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ExampleAPI.Tests.Installers
{
    public class SwaggerInstallerTests
    {
        private readonly ServiceCollection _services;
        private readonly IConfiguration _configuration;

        public SwaggerInstallerTests()
        {
            _services = new ServiceCollection();
            _configuration = new ConfigurationBuilder().Build();
        }

        [Fact]
        public void InstallServices_RegistersSwaggerGen()
        {
            var installer = new SwaggerInstaller();

            installer.InstallServices(_services, _configuration);

            _services.Should().Contain(sd =>
                sd.ServiceType.FullName.Contains("Swagger"));
        }

        [Fact]
        public void InstallServices_RegistersSwaggerExampleProviders()
        {
            var installer = new SwaggerInstaller();

            installer.InstallServices(_services, _configuration);

            _services.Should().Contain(sd =>
                sd.ServiceType.FullName.Contains("Example"));
        }
    }
}
