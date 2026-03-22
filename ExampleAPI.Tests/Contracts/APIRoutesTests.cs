using FluentAssertions;
using Xunit;
using V1Routes = ExampleAPI.Contracts.V1.APIRoutes;
using V2Routes = ExampleAPI.Contracts.V2.APIRoutes;

namespace ExampleAPI.Tests.Contracts
{
    public class APIRoutesV1Tests
    {
        [Fact]
        public void Weather_Get_ShouldContainV1()
        {
            V1Routes.Weather.Get.Should().Contain("v1");
        }

        [Fact]
        public void Weather_AuthGet_ShouldContainV1()
        {
            V1Routes.Weather.AuthGet.Should().Contain("v1");
        }

        [Fact]
        public void Auth_UserLogin_ShouldHaveCorrectPath()
        {
            V1Routes.Auth.AuthUserLogin.Should().Be("api/v1/auth/UserLogin");
        }

        [Fact]
        public void Auth_APILogin_ShouldHaveCorrectPath()
        {
            V1Routes.Auth.AuthAPILogin.Should().Be("api/v1/auth/APILogin");
        }

        [Fact]
        public void Example_Route_ShouldHaveCorrectPath()
        {
            V1Routes.Example.Route.Should().Be("api/v1/example");
        }

        [Fact]
        public void Example_RouteById_ShouldContainIdParameter()
        {
            V1Routes.Example.RoutebyId.Should().Contain("{id}");
        }

        [Fact]
        public void AllRoutes_ShouldStartWithApi()
        {
            V1Routes.Weather.Get.Should().StartWith("api/");
            V1Routes.Weather.AuthGet.Should().StartWith("api/");
            V1Routes.Auth.AuthUserLogin.Should().StartWith("api/");
            V1Routes.Auth.AuthAPILogin.Should().StartWith("api/");
            V1Routes.Example.Route.Should().StartWith("api/");
            V1Routes.Example.RoutebyId.Should().StartWith("api/");
        }
    }

    public class APIRoutesV2Tests
    {
        [Fact]
        public void Weather_Get_ShouldContainV2()
        {
            V2Routes.Weather.Get.Should().Contain("v2");
        }

        [Fact]
        public void Auth_UserLogin_ShouldHaveCorrectPath()
        {
            V2Routes.Auth.AuthUserLogin.Should().Be("api/v2/auth/UserLogin");
        }

        [Fact]
        public void V2Routes_ShouldDifferFromV1()
        {
            V2Routes.Weather.Get.Should().NotBe(V1Routes.Weather.Get);
            V2Routes.Auth.AuthUserLogin.Should().NotBe(V1Routes.Auth.AuthUserLogin);
        }
    }
}
