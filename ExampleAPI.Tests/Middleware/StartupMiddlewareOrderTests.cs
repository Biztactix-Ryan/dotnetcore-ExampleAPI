using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace ExampleAPI.Tests.Middleware
{
    public class StartupMiddlewareOrderTests
    {
        [Fact]
        public void Configure_UseHttpsRedirection_ShouldBeCalledBeforeUseRouting()
        {
            // Read the Startup.Configure method source to verify middleware ordering.
            // We verify by checking the source file directly since the extension methods
            // on IApplicationBuilder are static and hard to intercept via mocks.
            var startupSource = File.ReadAllText(FindStartupFile());

            var httpsRedirectionIndex = startupSource.IndexOf("UseHttpsRedirection()");
            var useRoutingIndex = startupSource.IndexOf("UseRouting()");

            httpsRedirectionIndex.Should().BeGreaterThan(-1, "UseHttpsRedirection() should be present in Startup.cs");
            useRoutingIndex.Should().BeGreaterThan(-1, "UseRouting() should be present in Startup.cs");
            httpsRedirectionIndex.Should().BeLessThan(useRoutingIndex,
                "UseHttpsRedirection() should be called before UseRouting()");
        }

        [Fact]
        public void Configure_UseHttpsRedirection_ShouldBeCalledBeforeUseCors()
        {
            var startupSource = File.ReadAllText(FindStartupFile());

            var httpsRedirectionIndex = startupSource.IndexOf("UseHttpsRedirection()");
            var useCorsIndex = startupSource.IndexOf("UseCors(");

            httpsRedirectionIndex.Should().BeGreaterThan(-1, "UseHttpsRedirection() should be present in Startup.cs");
            useCorsIndex.Should().BeGreaterThan(-1, "UseCors() should be present in Startup.cs");
            httpsRedirectionIndex.Should().BeLessThan(useCorsIndex,
                "UseHttpsRedirection() should be called before UseCors()");
        }

        [Fact]
        public void Configure_UseHttpsRedirection_ShouldBeCalledBeforeAnyRoutingMiddleware()
        {
            // Verify that HTTPS redirection occurs before ALL routing-related middleware,
            // ensuring non-HTTPS requests are redirected before any routing occurs.
            var startupSource = File.ReadAllText(FindStartupFile());

            var httpsRedirectionIndex = startupSource.IndexOf("UseHttpsRedirection()");
            httpsRedirectionIndex.Should().BeGreaterThan(-1, "UseHttpsRedirection() should be present in Startup.cs");

            var routingMiddleware = new[]
            {
                ("UseRouting()", startupSource.IndexOf("UseRouting()")),
                ("UseCors(", startupSource.IndexOf("UseCors(")),
                ("UseEndpoints(", startupSource.IndexOf("UseEndpoints(")),
            };

            foreach (var (name, index) in routingMiddleware)
            {
                if (index > -1)
                {
                    httpsRedirectionIndex.Should().BeLessThan(index,
                        $"UseHttpsRedirection() must be called before {name} so non-HTTPS requests are redirected before any routing occurs");
                }
            }
        }

        private static string FindStartupFile()
        {
            // Walk up from the test assembly output directory to find the solution root
            var dir = AppContext.BaseDirectory;
            while (dir != null)
            {
                var candidate = Path.Combine(dir, "Startup.cs");
                if (File.Exists(candidate))
                    return candidate;

                // Also check if ExampleAPI.sln exists here (solution root)
                if (File.Exists(Path.Combine(dir, "ExampleAPI.sln")))
                {
                    candidate = Path.Combine(dir, "Startup.cs");
                    if (File.Exists(candidate))
                        return candidate;
                }

                dir = Directory.GetParent(dir)?.FullName;
            }

            throw new FileNotFoundException("Could not locate Startup.cs from base directory: " + AppContext.BaseDirectory);
        }
    }
}
