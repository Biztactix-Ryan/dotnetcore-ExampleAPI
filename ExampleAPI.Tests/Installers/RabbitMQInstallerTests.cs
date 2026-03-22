using ExampleAPI.Installers;
using ExampleAPI.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using Xunit;

namespace ExampleAPI.Tests.Installers
{
    public class RabbitMQInstallerTests
    {
        private readonly ServiceCollection _services;
        private readonly IConfiguration _configuration;

        public RabbitMQInstallerTests()
        {
            _services = new ServiceCollection();
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "rabbit:HostName", "localhost" },
                    { "rabbit:UserName", "guest" },
                    { "rabbit:Password", "guest" }
                })
                .Build();
        }

        [Fact]
        public void InstallServices_RegistersObjectPoolProvider()
        {
            var installer = new ExampleAPI.Installers.RabbitMQ();

            installer.InstallServices(_services, _configuration);

            _services.Should().Contain(sd =>
                sd.ServiceType == typeof(ObjectPoolProvider));
        }

        [Fact]
        public void InstallServices_RegistersRabbitModelPooledObjectPolicy()
        {
            var installer = new ExampleAPI.Installers.RabbitMQ();

            installer.InstallServices(_services, _configuration);

            _services.Should().Contain(sd =>
                sd.ServiceType == typeof(IPooledObjectPolicy<IModel>));
        }

        [Fact]
        public void InstallServices_RegistersRabbitManager()
        {
            var installer = new ExampleAPI.Installers.RabbitMQ();

            installer.InstallServices(_services, _configuration);

            _services.Should().Contain(sd =>
                sd.ServiceType == typeof(IRabbitManager));
        }
    }
}
