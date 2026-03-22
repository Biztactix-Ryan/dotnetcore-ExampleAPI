using System.Diagnostics;
using System.Reflection;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using ExampleAPI.Helpers;
using ExampleAPI.Models.ExampleXPOModel;
using FluentAssertions;
using Xunit;

namespace ExampleAPI.Tests.Helpers
{
    public class SeedDataHelperTests
    {
        private static UnitOfWork CreateInMemoryUnitOfWork()
        {
            var dict = new ReflectionDictionary();
            dict.GetDataStoreSchema(typeof(ExampleObject));
            var dataStore = new InMemoryDataStore(AutoCreateOption.DatabaseAndSchema);
            var dataLayer = new SimpleDataLayer(dict, dataStore);
            return new UnitOfWork(dataLayer);
        }

        [Fact]
        public void CreateCustomer_ShouldCreateExactlyOneRecord()
        {
            using var uow = CreateInMemoryUnitOfWork();

            var method = typeof(SeedDataHelper).GetMethod(
                "CreateCustomer",
                BindingFlags.NonPublic | BindingFlags.Static);
            method.Should().NotBeNull("CreateCustomer method should exist");

            method.Invoke(null, new object[] { uow, "John", "Doe" });
            uow.CommitChanges();

            var count = uow.Query<ExampleObject>().Count();
            count.Should().Be(1, "CreateCustomer should create exactly one record per call");
        }

        [Fact]
        public void CreateCustomer_CalledMultipleTimes_ShouldCreateOneRecordPerCall()
        {
            using var uow = CreateInMemoryUnitOfWork();

            var method = typeof(SeedDataHelper).GetMethod(
                "CreateCustomer",
                BindingFlags.NonPublic | BindingFlags.Static);
            method.Should().NotBeNull();

            method.Invoke(null, new object[] { uow, "John", "Doe" });
            method.Invoke(null, new object[] { uow, "Jane", "Smith" });
            method.Invoke(null, new object[] { uow, "Bob", "Jones" });
            uow.CommitChanges();

            var count = uow.Query<ExampleObject>().Count();
            count.Should().Be(3, "each CreateCustomer call should create exactly one record");
        }

        [Fact]
        public void Seed_ShouldCreate272Records_Not73984()
        {
            using var uow = CreateInMemoryUnitOfWork();

            SeedDataHelper.Seed(uow);

            var count = uow.Query<ExampleObject>().Count();
            count.Should().Be(17 * 16, "Seed should create 17 first names * 16 last names = 272 records");
        }
        [Fact]
        public void Seed_ShouldCompleteWithinReasonableTime()
        {
            using var uow = CreateInMemoryUnitOfWork();

            var stopwatch = Stopwatch.StartNew();
            SeedDataHelper.Seed(uow);
            stopwatch.Stop();

            stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000,
                "seeding 272 records should complete well under 5 seconds");
        }

        [Fact]
        public void CreateCustomer_ShouldSetFirstNameAndLastName()
        {
            using var uow = CreateInMemoryUnitOfWork();

            var method = typeof(SeedDataHelper).GetMethod(
                "CreateCustomer",
                BindingFlags.NonPublic | BindingFlags.Static);
            method.Should().NotBeNull();

            method.Invoke(null, new object[] { uow, "Alice", "Wonder" });
            uow.CommitChanges();

            var customer = uow.Query<ExampleObject>().First();
            customer.FirstName.Should().Be("Alice");
            customer.LastName.Should().Be("Wonder");
        }

        [Fact]
        public void Seed_ShouldUseAllFirstNames()
        {
            using var uow = CreateInMemoryUnitOfWork();

            SeedDataHelper.Seed(uow);

            var expectedFirstNames = new[] {
                "Peter", "Ryan", "Richard", "Tom", "Mark", "Steve",
                "Jimmy", "Jeffrey", "Andrew", "Dave", "Bert", "Mike",
                "Ray", "Paul", "Brad", "Carl", "Jerry" };

            var actualFirstNames = uow.Query<ExampleObject>()
                .Select(c => c.FirstName)
                .Distinct()
                .OrderBy(n => n)
                .ToList();

            actualFirstNames.Should().BeEquivalentTo(expectedFirstNames,
                "all 17 first names should appear in seeded data");
        }

        [Fact]
        public void Seed_ShouldUseAllLastNames()
        {
            using var uow = CreateInMemoryUnitOfWork();

            SeedDataHelper.Seed(uow);

            var expectedLastNames = new[] {
                "Dolan", "Fischer", "Hamlett", "Hamilton", "Lee",
                "Lewis", "McClain", "Miller", "Murrel", "Parkins",
                "Roller", "Shipman", "Bailey", "Barnes", "Lucas", "Campbell" };

            var actualLastNames = uow.Query<ExampleObject>()
                .Select(c => c.LastName)
                .Distinct()
                .OrderBy(n => n)
                .ToList();

            actualLastNames.Should().BeEquivalentTo(expectedLastNames,
                "all 16 last names should appear in seeded data");
        }

        [Fact]
        public void Seed_CalledTwice_ShouldDoubleRecordCount()
        {
            using var uow = CreateInMemoryUnitOfWork();

            SeedDataHelper.Seed(uow);
            SeedDataHelper.Seed(uow);

            var count = uow.Query<ExampleObject>().Count();
            count.Should().Be(272 * 2,
                "Seed is not idempotent — calling twice creates duplicate records");
        }

        [Fact]
        public void InMemoryDataStore_ShouldIsolateTestData_BetweenUnitOfWorkInstances()
        {
            using var uow1 = CreateInMemoryUnitOfWork();
            using var uow2 = CreateInMemoryUnitOfWork();

            SeedDataHelper.Seed(uow1);

            var countInUow2 = uow2.Query<ExampleObject>().Count();
            countInUow2.Should().Be(0,
                "each test gets its own InMemoryDataStore — no data leaks between UnitOfWork instances");
        }

        [Fact]
        public void InMemoryDataStore_ShouldNotRequireExternalDatabase()
        {
            // Verify the mock data layer works without any connection string or external DB
            var dict = new ReflectionDictionary();
            dict.GetDataStoreSchema(typeof(ExampleObject));
            var dataStore = new InMemoryDataStore(AutoCreateOption.DatabaseAndSchema);
            var dataLayer = new SimpleDataLayer(dict, dataStore);

            using var uow = new UnitOfWork(dataLayer);
            SeedDataHelper.Seed(uow);

            var count = uow.Query<ExampleObject>().Count();
            count.Should().Be(272, "InMemoryDataStore fully replaces a real database for testing");
        }
    }
}
