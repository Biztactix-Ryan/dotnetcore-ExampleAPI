using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Xunit;
using V1Routes = ExampleAPI.Contracts.V1.APIRoutes;
using V2Routes = ExampleAPI.Contracts.V2.APIRoutes;
using V2Weather = ExampleAPI.Controllers.V2.WeatherForecastController;
using V2Auth = ExampleAPI.Controllers.V2.AuthController;

namespace ExampleAPI.Tests.Contracts.V2
{
    public class APIRoutesTests
    {
        // --- Value-based route validation ---

        [Fact]
        public void Weather_Get_ShouldHaveCorrectPath()
        {
            V2Routes.Weather.Get.Should().Be("api/v2/weather");
        }

        [Fact]
        public void Weather_AuthGet_ShouldHaveCorrectPath()
        {
            V2Routes.Weather.AuthGet.Should().Be("api/v2/weather/auth");
        }

        [Fact]
        public void Auth_UserLogin_ShouldHaveCorrectPath()
        {
            V2Routes.Auth.AuthUserLogin.Should().Be("api/v2/auth/UserLogin");
        }

        [Fact]
        public void Auth_APILogin_ShouldHaveCorrectPath()
        {
            V2Routes.Auth.AuthAPILogin.Should().Be("api/v2/auth/APILogin");
        }

        [Fact]
        public void AllRoutes_ShouldContainV2()
        {
            V2Routes.Weather.Get.Should().Contain("v2");
            V2Routes.Weather.AuthGet.Should().Contain("v2");
            V2Routes.Auth.AuthUserLogin.Should().Contain("v2");
            V2Routes.Auth.AuthAPILogin.Should().Contain("v2");
        }

        [Fact]
        public void AllRoutes_ShouldStartWithApi()
        {
            V2Routes.Weather.Get.Should().StartWith("api/");
            V2Routes.Weather.AuthGet.Should().StartWith("api/");
            V2Routes.Auth.AuthUserLogin.Should().StartWith("api/");
            V2Routes.Auth.AuthAPILogin.Should().StartWith("api/");
        }

        [Fact]
        public void V2Routes_ShouldDifferFromV1()
        {
            V2Routes.Weather.Get.Should().NotBe(V1Routes.Weather.Get);
            V2Routes.Weather.AuthGet.Should().NotBe(V1Routes.Weather.AuthGet);
            V2Routes.Auth.AuthUserLogin.Should().NotBe(V1Routes.Auth.AuthUserLogin);
            V2Routes.Auth.AuthAPILogin.Should().NotBe(V1Routes.Auth.AuthAPILogin);
        }

        // --- Reflection-based controller route validation ---

        [Fact]
        public void WeatherController_Get_HasHttpGetMatchingContract()
        {
            var method = typeof(V2Weather).GetMethod("Get");
            method.Should().NotBeNull("V2 WeatherForecastController must have a Get method");

            var attr = method.GetCustomAttribute<HttpGetAttribute>();
            attr.Should().NotBeNull("Get action must have [HttpGet]");
            attr.Template.Should().Be(V2Routes.Weather.Get,
                "route template must match V2 APIRoutes.Weather.Get contract");
        }

        [Fact]
        public void AuthController_UserLogin_HasHttpPostMatchingContract()
        {
            var method = GetMethodByRoute<V2Auth, HttpPostAttribute>(V2Routes.Auth.AuthUserLogin);
            method.Should().NotBeNull(
                $"V2 AuthController must have a POST action with route '{V2Routes.Auth.AuthUserLogin}'");
        }

        [Fact]
        public void AuthController_APILogin_HasHttpPostMatchingContract()
        {
            var method = GetMethodByRoute<V2Auth, HttpPostAttribute>(V2Routes.Auth.AuthAPILogin);
            method.Should().NotBeNull(
                $"V2 AuthController must have a POST action with route '{V2Routes.Auth.AuthAPILogin}'");
        }

        [Fact]
        public void AllV2Controllers_ShouldHaveApiControllerAttribute()
        {
            typeof(V2Weather).GetCustomAttribute<ApiControllerAttribute>()
                .Should().NotBeNull("V2 WeatherForecastController must have [ApiController]");

            typeof(V2Auth).GetCustomAttribute<ApiControllerAttribute>()
                .Should().NotBeNull("V2 AuthController must have [ApiController]");
        }

        [Fact]
        public void AllV2Actions_ShouldHaveExplicitRouteTemplates()
        {
            var controllerTypes = new[] { typeof(V2Weather), typeof(V2Auth) };

            foreach (var controllerType in controllerTypes)
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
                        attr.Template.Should().Contain("v2",
                            $"{controllerType.Name}.{action.Name} route must contain 'v2'");
                    }
                }
            }
        }

        [Theory]
        [InlineData("api/v2/weather")]
        [InlineData("api/v2/weather/auth")]
        [InlineData("api/v2/auth/UserLogin")]
        [InlineData("api/v2/auth/APILogin")]
        public void V2Routes_ShouldNotHaveLeadingSlash(string routeValue)
        {
            routeValue.Should().NotStartWith("/",
                "route must not start with '/' for endpoint routing");
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
