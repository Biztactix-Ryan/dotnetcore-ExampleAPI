using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using ExampleAPI.Helpers;
using ExampleAPI.Models.ExampleXPOModel;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace ExampleAPI.Tests.Helpers
{
    public class XPODemoDataTests
    {
        private static UnitOfWork CreateInMemoryUnitOfWork()
        {
            var dict = new ReflectionDictionary();
            dict.GetDataStoreSchema(typeof(ExampleObject));
            var dataStore = new InMemoryDataStore(AutoCreateOption.DatabaseAndSchema);
            var dataLayer = new SimpleDataLayer(dict, dataStore);
            return new UnitOfWork(dataLayer);
        }

        private static (Mock<IApplicationBuilder> appMock, UnitOfWork uow) SetupMocks()
        {
            var uow = CreateInMemoryUnitOfWork();

            var mockScope = new Mock<IServiceScope>();
            var mockScopeServiceProvider = new Mock<IServiceProvider>();
            mockScopeServiceProvider
                .Setup(sp => sp.GetService(typeof(UnitOfWork)))
                .Returns(uow);
            mockScope.Setup(s => s.ServiceProvider).Returns(mockScopeServiceProvider.Object);

            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            mockScopeFactory.Setup(f => f.CreateScope()).Returns(mockScope.Object);

            var mockRootServiceProvider = new Mock<IServiceProvider>();
            mockRootServiceProvider
                .Setup(sp => sp.GetService(typeof(IServiceScopeFactory)))
                .Returns(mockScopeFactory.Object);

            var appMock = new Mock<IApplicationBuilder>();
            appMock.Setup(a => a.ApplicationServices).Returns(mockRootServiceProvider.Object);

            return (appMock, uow);
        }

        [Fact]
        public void UseXpoDemoData_ShouldReturnSameApplicationBuilder()
        {
            var (appMock, uow) = SetupMocks();
            using (uow)
            {
                var result = appMock.Object.UseXpoDemoData();

                result.Should().BeSameAs(appMock.Object,
                    "extension method should return the same IApplicationBuilder for fluent chaining");
            }
        }

        [Fact]
        public void UseXpoDemoData_ShouldSeedDemoData()
        {
            var (appMock, uow) = SetupMocks();
            using (uow)
            {
                appMock.Object.UseXpoDemoData();

                var count = uow.Query<ExampleObject>().Count();
                count.Should().Be(17 * 16,
                    "UseXpoDemoData should call SeedDataHelper.Seed which creates 272 records");
            }
        }

        [Fact]
        public void UseXpoDemoData_ShouldCreateCustomersWithExpectedProperties()
        {
            var (appMock, uow) = SetupMocks();
            using (uow)
            {
                appMock.Object.UseXpoDemoData();

                var customers = uow.Query<ExampleObject>().ToList();
                customers.Should().AllSatisfy(c =>
                {
                    c.FirstName.Should().NotBeNullOrEmpty("each customer should have a first name");
                    c.LastName.Should().NotBeNullOrEmpty("each customer should have a last name");
                });
            }
        }

        [Fact]
        public void UseXpoDemoData_ShouldResolveUnitOfWorkFromServiceScope()
        {
            var uow = CreateInMemoryUnitOfWork();

            var mockScope = new Mock<IServiceScope>();
            var mockScopeServiceProvider = new Mock<IServiceProvider>();
            mockScopeServiceProvider
                .Setup(sp => sp.GetService(typeof(UnitOfWork)))
                .Returns(uow);
            mockScope.Setup(s => s.ServiceProvider).Returns(mockScopeServiceProvider.Object);

            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            mockScopeFactory.Setup(f => f.CreateScope()).Returns(mockScope.Object);

            var mockRootServiceProvider = new Mock<IServiceProvider>();
            mockRootServiceProvider
                .Setup(sp => sp.GetService(typeof(IServiceScopeFactory)))
                .Returns(mockScopeFactory.Object);

            var appMock = new Mock<IApplicationBuilder>();
            appMock.Setup(a => a.ApplicationServices).Returns(mockRootServiceProvider.Object);

            appMock.Object.UseXpoDemoData();

            // Verify the scope factory was used to create a scope
            mockScopeFactory.Verify(f => f.CreateScope(), Times.Once);
            // Verify UnitOfWork was resolved from the scope's service provider
            mockScopeServiceProvider.Verify(sp => sp.GetService(typeof(UnitOfWork)), Times.Once);

            uow.Dispose();
        }

        [Fact]
        public void UseXpoDemoData_ShouldDisposeScope()
        {
            var uow = CreateInMemoryUnitOfWork();

            var mockScope = new Mock<IServiceScope>();
            var mockScopeServiceProvider = new Mock<IServiceProvider>();
            mockScopeServiceProvider
                .Setup(sp => sp.GetService(typeof(UnitOfWork)))
                .Returns(uow);
            mockScope.Setup(s => s.ServiceProvider).Returns(mockScopeServiceProvider.Object);

            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            mockScopeFactory.Setup(f => f.CreateScope()).Returns(mockScope.Object);

            var mockRootServiceProvider = new Mock<IServiceProvider>();
            mockRootServiceProvider
                .Setup(sp => sp.GetService(typeof(IServiceScopeFactory)))
                .Returns(mockScopeFactory.Object);

            var appMock = new Mock<IApplicationBuilder>();
            appMock.Setup(a => a.ApplicationServices).Returns(mockRootServiceProvider.Object);

            appMock.Object.UseXpoDemoData();

            // Verify the scope is disposed (using statement in source)
            mockScope.Verify(s => s.Dispose(), Times.Once);

            uow.Dispose();
        }
    }
}
