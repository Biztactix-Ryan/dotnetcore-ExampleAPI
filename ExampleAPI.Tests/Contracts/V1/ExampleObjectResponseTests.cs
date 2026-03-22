using System.Text.Json;
using System.Text.Json.Serialization;
using ExampleAPI.Contracts.V1;
using FluentAssertions;
using Xunit;

namespace ExampleAPI.Tests.Contracts.V1
{
    public class ExampleObjectResponseTests
    {
        [Fact]
        public void Properties_ShouldBeSettable()
        {
            var obj = new ExampleObjectResponse
            {
                oid = 42,
                FirstName = "John",
                LastName = "Doe",
                Phone = "555-1234"
            };

            obj.oid.Should().Be(42);
            obj.FirstName.Should().Be("John");
            obj.LastName.Should().Be("Doe");
            obj.Phone.Should().Be("555-1234");
        }

        [Fact]
        public void Properties_DefaultValues()
        {
            var obj = new ExampleObjectResponse();

            obj.oid.Should().Be(0);
            obj.FirstName.Should().BeNull();
            obj.LastName.Should().BeNull();
            obj.Phone.Should().BeNull();
        }

        [Fact]
        public void ShouldHaveFourProperties()
        {
            var properties = typeof(ExampleObjectResponse).GetProperties();

            properties.Should().HaveCount(4);
        }

        [Fact]
        public void Oid_ShouldBeIntType()
        {
            var oidProp = typeof(ExampleObjectResponse).GetProperty("oid");

            oidProp.PropertyType.Should().Be(typeof(int));
        }

        [Fact]
        public void StringProperties_ShouldBeStringType()
        {
            var stringProps = new[] { "FirstName", "LastName", "Phone" };

            foreach (var propName in stringProps)
            {
                var prop = typeof(ExampleObjectResponse).GetProperty(propName);
                prop.PropertyType.Should().Be(typeof(string));
            }
        }

        [Fact]
        public void Oid_ShouldSerializeAsId_ViaJsonPropertyName()
        {
            var obj = new ExampleObjectResponse
            {
                oid = 99,
                FirstName = "Test",
                LastName = "User",
                Phone = "123-456"
            };

            var json = JsonSerializer.Serialize(obj);

            json.Should().Contain("\"id\":");
            json.Should().NotContain("\"oid\":");
        }

        [Fact]
        public void ShouldDeserializeFromJsonWithIdProperty()
        {
            var json = "{\"id\":55,\"FirstName\":\"Jane\",\"LastName\":\"Doe\",\"Phone\":\"555-0000\"}";

            var deserialized = JsonSerializer.Deserialize<ExampleObjectResponse>(json);

            deserialized.oid.Should().Be(55);
            deserialized.FirstName.Should().Be("Jane");
            deserialized.LastName.Should().Be("Doe");
            deserialized.Phone.Should().Be("555-0000");
        }

        [Fact]
        public void ShouldRoundTripSerialize()
        {
            var obj = new ExampleObjectResponse
            {
                oid = 10,
                FirstName = "Round",
                LastName = "Trip",
                Phone = "000-0000"
            };

            var json = JsonSerializer.Serialize(obj);
            var deserialized = JsonSerializer.Deserialize<ExampleObjectResponse>(json);

            deserialized.oid.Should().Be(10);
            deserialized.FirstName.Should().Be("Round");
            deserialized.LastName.Should().Be("Trip");
            deserialized.Phone.Should().Be("000-0000");
        }

        [Fact]
        public void Oid_ShouldHaveJsonPropertyNameAttribute()
        {
            var oidProp = typeof(ExampleObjectResponse).GetProperty("oid");
            var attr = oidProp.GetCustomAttributes(typeof(JsonPropertyNameAttribute), false)
                .Cast<JsonPropertyNameAttribute>()
                .FirstOrDefault();

            attr.Should().NotBeNull();
            attr.Name.Should().Be("id");
        }
    }
}
