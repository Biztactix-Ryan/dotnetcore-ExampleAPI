using AutoMapper;
using ExampleAPI.Mappers;
using FluentAssertions;
using Xunit;

namespace ExampleAPI.Tests.Mappers
{
    public class V1MappersTests
    {
        private readonly IMapper _mapper;

        public V1MappersTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<V1Mappers>());
            _mapper = config.CreateMapper();
        }

        [Fact]
        public void MapperConfiguration_ShouldBeValid()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<V1Mappers>());
            config.AssertConfigurationIsValid();
        }

        [Fact]
        public void Profile_ShouldCreateWithoutErrors()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<V1Mappers>());
            var mapper = config.CreateMapper();
            mapper.Should().NotBeNull();
        }
    }
}
