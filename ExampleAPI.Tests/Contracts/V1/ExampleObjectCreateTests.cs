using System.Text.Json;
using ExampleAPI.Contracts.V1;
using FluentAssertions;
using Xunit;

namespace ExampleAPI.Tests.Contracts.V1
{
    public class ExampleObjectCreateTests
    {
        [Fact]
        public void Properties_ShouldBeSettable()
        {
            var obj = new ExampleObjectCreate
            {
                FirstName = "John",
                LastName = "Doe",
                Phone = "555-1234"
            };

            obj.FirstName.Should().Be("John");
            obj.LastName.Should().Be("Doe");
            obj.Phone.Should().Be("555-1234");
        }

        [Fact]
        public void Properties_DefaultValues_ShouldBeNull()
        {
            var obj = new ExampleObjectCreate();

            obj.FirstName.Should().BeNull();
            obj.LastName.Should().BeNull();
            obj.Phone.Should().BeNull();
        }

        [Fact]
        public void ShouldHaveThreeProperties()
        {
            var properties = typeof(ExampleObjectCreate).GetProperties();

            properties.Should().HaveCount(3);
        }

        [Fact]
        public void Properties_ShouldBeStringType()
        {
            var properties = typeof(ExampleObjectCreate).GetProperties();

            foreach (var prop in properties)
            {
                prop.PropertyType.Should().Be(typeof(string));
            }
        }

        [Fact]
        public void ShouldSerializeToJson()
        {
            var obj = new ExampleObjectCreate
            {
                FirstName = "Jane",
                LastName = "Smith",
                Phone = "555-9876"
            };

            var json = JsonSerializer.Serialize(obj);
            var deserialized = JsonSerializer.Deserialize<ExampleObjectCreate>(json);

            deserialized.FirstName.Should().Be("Jane");
            deserialized.LastName.Should().Be("Smith");
            deserialized.Phone.Should().Be("555-9876");
        }

        [Fact]
        public void ShouldHaveExpectedPropertyNames()
        {
            var propertyNames = typeof(ExampleObjectCreate).GetProperties()
                .Select(p => p.Name)
                .ToList();

            propertyNames.Should().Contain("FirstName");
            propertyNames.Should().Contain("LastName");
            propertyNames.Should().Contain("Phone");
        }
    }
}
