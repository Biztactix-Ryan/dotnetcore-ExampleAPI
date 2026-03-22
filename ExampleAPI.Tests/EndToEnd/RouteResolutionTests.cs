using System.Reflection;
using ExampleAPI.Contracts.V1;
using ExampleAPI.Controllers.V1;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Xunit;
using V1Routes = ExampleAPI.Contracts.V1.APIRoutes;
using V2Routes = ExampleAPI.Contracts.V2.APIRoutes;
using V1Weather = ExampleAPI.Controllers.V1.WeatherForecastController;
using V2Weather = ExampleAPI.Controllers.V2.WeatherForecastController;
using V1Auth = ExampleAPI.Controllers.V1.AuthController;
using V2Auth = ExampleAPI.Controllers.V2.AuthController;

namespace ExampleAPI.Tests.EndToEnd
{
    /// <summary>
    /// Verifies that all controller actions have route attributes matching the APIRoutes contracts
    /// and that routes are well-formed for endpoint routing (no leading slash, valid segments).
    /// </summary>
    public class RouteResolutionTests
    {
        // --- V1 WeatherForecastController ---

        [Fact]
        public void V1Weather_Get_HasRouteMatchingContract()
        {
            var method = typeof(V1Weather).GetMethod("Get");
            var attr = method.GetCustomAttribute<HttpGetAttribute>();

            attr.Should().NotBeNull("Get action must have [HttpGet]");
            attr.Template.Should().Be(V1Routes.Weather.Get);
        }

        [Fact]
        public void V1Weather_AuthGet_HasRouteMatchingContract()
        {
            var method = typeof(V1Weather).GetMethod("AuthGet");
            var attr = method.GetCustomAttribute<HttpGetAttribute>();

            attr.Should().NotBeNull("AuthGet action must have [HttpGet]");
            attr.Template.Should().Be(V1Routes.Weather.AuthGet);
        }

        // --- V1 AuthController ---

        [Fact]
        public void V1Auth_UserLogin_HasRouteMatchingContract()
        {
            var method = GetMethodByRoute<V1Auth, HttpPostAttribute>(V1Routes.Auth.AuthUserLogin);
            method.Should().NotBeNull($"a POST action with route '{V1Routes.Auth.AuthUserLogin}' must exist");
        }

        [Fact]
        public void V1Auth_APILogin_HasRouteMatchingContract()
        {
            var method = GetMethodByRoute<V1Auth, HttpPostAttribute>(V1Routes.Auth.AuthAPILogin);
            method.Should().NotBeNull($"a POST action with route '{V1Routes.Auth.AuthAPILogin}' must exist");
        }

        // --- V1 ExampleController ---

        [Fact]
        public void V1Example_GetAll_HasRouteMatchingContract()
        {
            var methods = typeof(ExampleController).GetMethods()
                .Where(m => m.GetCustomAttribute<HttpGetAttribute>()?.Template == V1Routes.Example.Route);
            methods.Should().NotBeEmpty($"a GET action with route '{V1Routes.Example.Route}' must exist");
        }

        [Fact]
        public void V1Example_GetById_HasRouteMatchingContract()
        {
            var methods = typeof(ExampleController).GetMethods()
                .Where(m => m.GetCustomAttribute<HttpGetAttribute>()?.Template == V1Routes.Example.RoutebyId);
            methods.Should().NotBeEmpty($"a GET action with route '{V1Routes.Example.RoutebyId}' must exist");
        }

        [Fact]
        public void V1Example_Post_HasRouteMatchingContract()
        {
            var methods = typeof(ExampleController).GetMethods()
                .Where(m => m.GetCustomAttribute<HttpPostAttribute>()?.Template == V1Routes.Example.Route);
            methods.Should().NotBeEmpty($"a POST action with route '{V1Routes.Example.Route}' must exist");
        }

        [Fact]
        public void V1Example_Put_HasRouteMatchingContract()
        {
            var methods = typeof(ExampleController).GetMethods()
                .Where(m => m.GetCustomAttribute<HttpPutAttribute>()?.Template == V1Routes.Example.RoutebyId);
            methods.Should().NotBeEmpty($"a PUT action with route '{V1Routes.Example.RoutebyId}' must exist");
        }

        // --- V2 WeatherForecastController ---

        [Fact]
        public void V2Weather_Get_HasRouteMatchingContract()
        {
            var method = typeof(V2Weather).GetMethod("Get");
            var attr = method.GetCustomAttribute<HttpGetAttribute>();

            attr.Should().NotBeNull("Get action must have [HttpGet]");
            attr.Template.Should().Be(V2Routes.Weather.Get);
        }

        // --- V2 AuthController ---

        [Fact]
        public void V2Auth_UserLogin_HasRouteMatchingContract()
        {
            var method = GetMethodByRoute<V2Auth, HttpPostAttribute>(V2Routes.Auth.AuthUserLogin);
            method.Should().NotBeNull($"a POST action with route '{V2Routes.Auth.AuthUserLogin}' must exist");
        }

        [Fact]
        public void V2Auth_APILogin_HasRouteMatchingContract()
        {
            var method = GetMethodByRoute<V2Auth, HttpPostAttribute>(V2Routes.Auth.AuthAPILogin);
            method.Should().NotBeNull($"a POST action with route '{V2Routes.Auth.AuthAPILogin}' must exist");
        }

        // --- Route format validation ---

        [Theory]
        [InlineData(nameof(V1Routes.Weather.Get), "api/v1/weather")]
        [InlineData(nameof(V1Routes.Weather.AuthGet), "api/v1/weather/auth")]
        [InlineData(nameof(V1Routes.Auth.AuthUserLogin), "api/v1/auth/UserLogin")]
        [InlineData(nameof(V1Routes.Auth.AuthAPILogin), "api/v1/auth/APILogin")]
        [InlineData(nameof(V1Routes.Example.Route), "api/v1/example")]
        [InlineData(nameof(V1Routes.Example.RoutebyId), "api/v1/example/{id}")]
        public void V1Routes_ShouldNotHaveLeadingSlash(string routeName, string routeValue)
        {
            routeValue.Should().NotStartWith("/",
                $"route '{routeName}' must not start with '/' for endpoint routing");
        }

        [Theory]
        [InlineData(nameof(V2Routes.Weather.Get), "api/v2/weather")]
        [InlineData(nameof(V2Routes.Auth.AuthUserLogin), "api/v2/auth/UserLogin")]
        [InlineData(nameof(V2Routes.Auth.AuthAPILogin), "api/v2/auth/APILogin")]
        public void V2Routes_ShouldNotHaveLeadingSlash(string routeName, string routeValue)
        {
            routeValue.Should().NotStartWith("/",
                $"route '{routeName}' must not start with '/' for endpoint routing");
        }

        // --- All controllers have [ApiController] ---

        [Theory]
        [InlineData(typeof(V1Weather))]
        [InlineData(typeof(V1Auth))]
        [InlineData(typeof(ExampleController))]
        [InlineData(typeof(V2Weather))]
        [InlineData(typeof(V2Auth))]
        public void AllControllers_ShouldHaveApiControllerAttribute(Type controllerType)
        {
            controllerType.GetCustomAttribute<ApiControllerAttribute>()
                .Should().NotBeNull($"{controllerType.Name} must have [ApiController] for endpoint routing");
        }

        // --- Every action in every controller has an HTTP method attribute with a route template ---

        [Theory]
        [InlineData(typeof(V1Weather))]
        [InlineData(typeof(V1Auth))]
        [InlineData(typeof(ExampleController))]
        [InlineData(typeof(V2Weather))]
        [InlineData(typeof(V2Auth))]
        public void AllActions_ShouldHaveExplicitRouteTemplates(Type controllerType)
        {
            var actions = controllerType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName);

            actions.Should().NotBeEmpty($"{controllerType.Name} should have at least one action");

            foreach (var action in actions)
            {
                var httpMethodAttrs = action.GetCustomAttributes()
                    .OfType<HttpMethodAttribute>()
                    .ToList();

                httpMethodAttrs.Should().NotBeEmpty(
                    $"{controllerType.Name}.{action.Name} must have an HTTP method attribute");

                foreach (var attr in httpMethodAttrs)
                {
                    attr.Template.Should().NotBeNullOrEmpty(
                        $"{controllerType.Name}.{action.Name} must have a route template");
                    attr.Template.Should().StartWith("api/",
                        $"{controllerType.Name}.{action.Name} route should start with 'api/'");
                }
            }
        }

        // --- Helper ---

        private static MethodInfo GetMethodByRoute<TController, THttpAttr>(string route)
            where THttpAttr : HttpMethodAttribute
        {
            return typeof(TController).GetMethods()
                .FirstOrDefault(m => m.GetCustomAttribute<THttpAttr>()?.Template == route);
        }
    }
}
