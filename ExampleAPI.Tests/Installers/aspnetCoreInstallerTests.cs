using ExampleAPI.Installers;
using FluentAssertions;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Xunit;

namespace ExampleAPI.Tests.Installers
{
    public class aspnetCoreInstallerTests
    {
        private readonly ServiceCollection _services;
        private readonly IConfiguration _configuration;

        public aspnetCoreInstallerTests()
        {
            _services = new ServiceCollection();
            _configuration = new ConfigurationBuilder().Build();
        }

        [Fact]
        public void InstallServices_RegistersMvc()
        {
            var installer = new aspnetCoreInstaller();

            installer.InstallServices(_services, _configuration);

            _services.Should().Contain(sd => sd.ServiceType.FullName.Contains("Mvc"));
        }

        [Fact]
        public void InstallServices_RegistersCors()
        {
            var installer = new aspnetCoreInstaller();

            installer.InstallServices(_services, _configuration);

            _services.Should().Contain(sd =>
                sd.ServiceType.FullName.Contains("Cors"));
        }

        [Fact]
        public void InstallServices_RegistersMetrics()
        {
            var installer = new aspnetCoreInstaller();

            installer.InstallServices(_services, _configuration);

            _services.Should().Contain(sd =>
                sd.ServiceType.FullName.Contains("Metrics"));
        }

        [Fact]
        public void InstallServices_ConfiguresCorsOriginsFromAppsettings()
        {
            var services = new ServiceCollection();
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Cors:AllowedOrigins:0", "https://app.example.com" },
                    { "Cors:AllowedOrigins:1", "https://admin.example.com" }
                })
                .Build();

            var installer = new aspnetCoreInstaller();
            installer.InstallServices(services, config);

            var provider = services.BuildServiceProvider();
            var corsOptions = provider.GetRequiredService<IOptions<CorsOptions>>().Value;

            var nncorsPolicy = corsOptions.GetPolicy("NNCors");
            nncorsPolicy.Should().NotBeNull();
            nncorsPolicy.Origins.Should().Contain("https://app.example.com");
            nncorsPolicy.Origins.Should().Contain("https://admin.example.com");
            nncorsPolicy.AllowAnyOrigin.Should().BeFalse();

            var defaultPolicy = corsOptions.DefaultPolicyName;
            var defaultPolicyObj = corsOptions.GetPolicy(defaultPolicy);
            defaultPolicyObj.Should().NotBeNull();
            defaultPolicyObj.Origins.Should().Contain("https://app.example.com");
            defaultPolicyObj.Origins.Should().Contain("https://admin.example.com");
            defaultPolicyObj.AllowAnyOrigin.Should().BeFalse();
        }

        [Fact]
        public void InstallServices_CorsRejectsUnapprovedOrigins()
        {
            var services = new ServiceCollection();
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Cors:AllowedOrigins:0", "https://app.example.com" }
                })
                .Build();

            var installer = new aspnetCoreInstaller();
            installer.InstallServices(services, config);

            var provider = services.BuildServiceProvider();
            var corsOptions = provider.GetRequiredService<IOptions<CorsOptions>>().Value;

            var nncorsPolicy = corsOptions.GetPolicy("NNCors");
            nncorsPolicy.AllowAnyOrigin.Should().BeFalse();
            nncorsPolicy.Origins.Should().Contain("https://app.example.com");
            nncorsPolicy.Origins.Should().NotContain("https://evil.com",
                "only approved origins should be allowed");

            var defaultPolicy = corsOptions.GetPolicy(corsOptions.DefaultPolicyName);
            defaultPolicy.AllowAnyOrigin.Should().BeFalse();
            defaultPolicy.Origins.Should().Contain("https://app.example.com");
            defaultPolicy.Origins.Should().NotContain("https://evil.com",
                "only approved origins should be allowed");
        }

        [Fact]
        public void InstallServices_CorsAllowsOnlyConfiguredOrigins()
        {
            var services = new ServiceCollection();
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Cors:AllowedOrigins:0", "https://app.example.com" },
                    { "Cors:AllowedOrigins:1", "https://admin.example.com" }
                })
                .Build();

            var installer = new aspnetCoreInstaller();
            installer.InstallServices(services, config);

            var provider = services.BuildServiceProvider();
            var corsOptions = provider.GetRequiredService<IOptions<CorsOptions>>().Value;

            var nncorsPolicy = corsOptions.GetPolicy("NNCors");
            nncorsPolicy.Origins.Should().HaveCount(2,
                "only the configured origins should be present");
            nncorsPolicy.Origins.Should().BeEquivalentTo(
                new[] { "https://app.example.com", "https://admin.example.com" });

            var defaultPolicy = corsOptions.GetPolicy(corsOptions.DefaultPolicyName);
            defaultPolicy.Origins.Should().HaveCount(2,
                "only the configured origins should be present");
            defaultPolicy.Origins.Should().BeEquivalentTo(
                new[] { "https://app.example.com", "https://admin.example.com" });
        }

        [Fact]
        public void InstallServices_ProductionConfigDoesNotAllowAnyOrigin()
        {
            var services = new ServiceCollection();
            var config = new ConfigurationBuilder()
                .SetBasePath(System.IO.Path.GetFullPath(
                    System.IO.Path.Combine(System.AppContext.BaseDirectory, "..", "..", "..", "..")))
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var installer = new aspnetCoreInstaller();
            installer.InstallServices(services, config);

            var provider = services.BuildServiceProvider();
            var corsOptions = provider.GetRequiredService<IOptions<CorsOptions>>().Value;

            var nncorsPolicy = corsOptions.GetPolicy("NNCors");
            nncorsPolicy.Should().NotBeNull();
            nncorsPolicy.AllowAnyOrigin.Should().BeFalse(
                "production configuration must not allow any origin");

            var defaultPolicy = corsOptions.GetPolicy(corsOptions.DefaultPolicyName);
            defaultPolicy.Should().NotBeNull();
            defaultPolicy.AllowAnyOrigin.Should().BeFalse(
                "production configuration must not allow any origin");
        }

        [Fact]
        public void InstallServices_DoesNotDisableEndpointRouting()
        {
            // Startup.Configure() uses UseEndpoints() which requires endpoint routing.
            // The installer must not set EnableEndpointRouting = false.
            var installer = new aspnetCoreInstaller();

            installer.InstallServices(_services, _configuration);

            var provider = _services.BuildServiceProvider();
            var mvcOptions = provider.GetRequiredService<IOptions<MvcOptions>>().Value;
            mvcOptions.EnableEndpointRouting.Should().BeTrue(
                "Startup uses UseEndpoints() which requires endpoint routing to be enabled");
        }
    }
}
