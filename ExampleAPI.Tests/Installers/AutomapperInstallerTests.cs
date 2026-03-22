using AutoMapper;
using ExampleAPI.Installers;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ExampleAPI.Tests.Installers
{
    public class AutomapperInstallerTests
    {
        private readonly ServiceCollection _services;
        private readonly IConfiguration _configuration;

        public AutomapperInstallerTests()
        {
            _services = new ServiceCollection();
            _configuration = new ConfigurationBuilder().Build();
        }

        [Fact]
        public void InstallServices_RegistersAutoMapper()
        {
            var installer = new AutomapperInstaller();

            installer.InstallServices(_services, _configuration);

            _services.Should().Contain(sd => sd.ServiceType == typeof(IMapper));
        }

        [Fact]
        public void InstallServices_RegistersMapperConfiguration()
        {
            var installer = new AutomapperInstaller();

            installer.InstallServices(_services, _configuration);

            _services.Should().Contain(sd =>
                sd.ServiceType == typeof(AutoMapper.IConfigurationProvider));
        }
    }
}
