using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using ExampleAPI.Models.ExampleXPOModel;
using FluentAssertions;
using Xunit;

namespace ExampleAPI.Tests.Models
{
    public class ExampleObjectTests : IDisposable
    {
        private readonly UnitOfWork _uow;

        public ExampleObjectTests()
        {
            var dataStore = new InMemoryDataStore(AutoCreateOption.DatabaseAndSchema);
            var dataLayer = new SimpleDataLayer(dataStore);
            _uow = new UnitOfWork(dataLayer);
        }

        public void Dispose()
        {
            _uow?.Dispose();
        }

        [Fact]
        public void Constructor_WithSession_ShouldCreateInstance()
        {
            var obj = new ExampleObject(_uow);

            obj.Should().NotBeNull();
            obj.Session.Should().Be(_uow);
        }

        [Fact]
        public void FirstName_ShouldBeSettableAndGettable()
        {
            var obj = new ExampleObject(_uow);

            obj.FirstName = "John";

            obj.FirstName.Should().Be("John");
        }

        [Fact]
        public void LastName_ShouldBeSettableAndGettable()
        {
            var obj = new ExampleObject(_uow);

            obj.LastName = "Doe";

            obj.LastName.Should().Be("Doe");
        }

        [Fact]
        public void Phone_ShouldBeSettableAndGettable()
        {
            var obj = new ExampleObject(_uow);

            obj.Phone = "555-1234";

            obj.Phone.Should().Be("555-1234");
        }

        [Fact]
        public void Properties_ShouldAllBeSettable()
        {
            var obj = new ExampleObject(_uow)
            {
                FirstName = "Jane",
                LastName = "Smith",
                Phone = "555-9876"
            };

            obj.FirstName.Should().Be("Jane");
            obj.LastName.Should().Be("Smith");
            obj.Phone.Should().Be("555-9876");
        }

        [Fact]
        public void Properties_ShouldDefaultToNull()
        {
            var obj = new ExampleObject(_uow);

            obj.FirstName.Should().BeNull();
            obj.LastName.Should().BeNull();
            obj.Phone.Should().BeNull();
        }

        [Fact]
        public void ShouldInheritFromXPObject()
        {
            var obj = new ExampleObject(_uow);

            obj.Should().BeAssignableTo<XPObject>();
        }

        [Fact]
        public void SetPropertyValue_ShouldTrackChanges()
        {
            var obj = new ExampleObject(_uow);
            _uow.CommitChanges();

            obj.FirstName = "Updated";

            _uow.GetObjectsToSave().Count.Should().Be(1);
        }

        [Fact]
        public void ShouldPersistAndRetrieveFromDataStore()
        {
            var obj = new ExampleObject(_uow)
            {
                FirstName = "Test",
                LastName = "User",
                Phone = "123-456-7890"
            };
            _uow.CommitChanges();

            using var newUow = new UnitOfWork(_uow.DataLayer);
            var retrieved = newUow.FindObject<ExampleObject>(null);

            retrieved.Should().NotBeNull();
            retrieved.FirstName.Should().Be("Test");
            retrieved.LastName.Should().Be("User");
            retrieved.Phone.Should().Be("123-456-7890");
        }

        [Fact]
        public void AfterConstruction_ShouldBeCalledDuringCreation()
        {
            // AfterConstruction is called automatically by XPO during object creation.
            // Verify the object is in a valid state after construction completes.
            var obj = new ExampleObject(_uow);

            obj.Should().NotBeNull();
            obj.FirstName.Should().BeNull();
            obj.LastName.Should().BeNull();
            obj.Phone.Should().BeNull();
            obj.Session.Should().Be(_uow);
        }

        [Fact]
        public void SetPropertyValue_ShouldUpdateMultipleProperties()
        {
            var obj = new ExampleObject(_uow);
            _uow.CommitChanges();

            obj.FirstName = "NewFirst";
            obj.LastName = "NewLast";
            obj.Phone = "555-0000";

            obj.FirstName.Should().Be("NewFirst");
            obj.LastName.Should().Be("NewLast");
            obj.Phone.Should().Be("555-0000");
            _uow.GetObjectsToSave().Count.Should().Be(1);
        }

        [Fact]
        public void SetPropertyValue_ShouldNotMarkDirtyWhenSameValueSet()
        {
            var obj = new ExampleObject(_uow) { FirstName = "Same" };
            _uow.CommitChanges();

            obj.FirstName = "Same";

            _uow.GetObjectsToSave().Count.Should().Be(0);
        }

        [Fact]
        public void ClassMetadata_ShouldHaveExpectedProperties()
        {
            var classInfo = _uow.GetClassInfo<ExampleObject>();

            classInfo.FindMember("FirstName").Should().NotBeNull();
            classInfo.FindMember("LastName").Should().NotBeNull();
            classInfo.FindMember("Phone").Should().NotBeNull();
        }
    }
}
