using AutoMapper;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using ExampleAPI.Contracts.V1;
using ExampleAPI.Controllers.V1;
using ExampleAPI.Models.ExampleXPOModel;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ExampleAPI.Tests.Controllers
{
    public class ExampleControllerTests : IDisposable
    {
        private readonly Mock<IMapper> _mockMapper;
        private readonly UnitOfWork _uow;
        private readonly ExampleController _controller;

        public ExampleControllerTests()
        {
            _mockMapper = new Mock<IMapper>();
            var dict = new ReflectionDictionary();
            dict.GetDataStoreSchema(typeof(ExampleObject));
            var dataStore = new InMemoryDataStore(AutoCreateOption.DatabaseAndSchema);
            var dataLayer = new SimpleDataLayer(dict, dataStore);
            _uow = new UnitOfWork(dataLayer);
            _controller = new ExampleController(_mockMapper.Object, _uow);
        }

        public void Dispose()
        {
            _uow?.Dispose();
        }

        [Fact]
        public void Get_ShouldReturnMappedExampleObjects()
        {
            var expectedResponses = new List<ExampleObjectResponse>
            {
                new ExampleObjectResponse { oid = 1, FirstName = "John", LastName = "Doe", Phone = "1234567890" },
                new ExampleObjectResponse { oid = 2, FirstName = "Jane", LastName = "Smith", Phone = "0987654321" }
            };

            _mockMapper
                .Setup(m => m.Map<List<ExampleObjectResponse>>(It.IsAny<IQueryable<ExampleObject>>()))
                .Returns(expectedResponses);

            var result = _controller.Get().ToList();

            result.Should().HaveCount(2);
            result[0].FirstName.Should().Be("John");
            result[1].FirstName.Should().Be("Jane");
        }

        [Fact]
        public void Get_ShouldReturnEmptyListWhenNoObjects()
        {
            _mockMapper
                .Setup(m => m.Map<List<ExampleObjectResponse>>(It.IsAny<IQueryable<ExampleObject>>()))
                .Returns(new List<ExampleObjectResponse>());

            var result = _controller.Get().ToList();

            result.Should().BeEmpty();
        }

        [Fact]
        public void GetById_ShouldReturnMappedObject()
        {
            var exampleObj = new ExampleObject(_uow)
            {
                FirstName = "John",
                LastName = "Doe",
                Phone = "1234567890"
            };
            exampleObj.Save();
            _uow.CommitChanges();

            var expectedResponse = new ExampleObjectResponse
            {
                oid = exampleObj.Oid,
                FirstName = "John",
                LastName = "Doe",
                Phone = "1234567890"
            };

            _mockMapper
                .Setup(m => m.Map<ExampleObjectResponse>(It.IsAny<ExampleObject>()))
                .Returns(expectedResponse);

            var result = _controller.Get(exampleObj.Oid) as OkObjectResult;

            result.Should().NotBeNull();
            var response = result.Value as ExampleObjectResponse;
            response.FirstName.Should().Be("John");
            response.oid.Should().Be(exampleObj.Oid);
        }

        [Fact]
        public void GetById_ShouldReturnNotFound_WhenObjectDoesNotExist()
        {
            var result = _controller.Get(999);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void GetById_ShouldNotThrowNullReferenceException_WhenObjectDoesNotExist()
        {
            var act = () => _controller.Get(999);

            act.Should().NotThrow<NullReferenceException>();
        }

        [Fact]
        public async Task Put_ShouldReturnBadRequest_WhenObjectNotFound()
        {
            var result = await _controller.Put(999, new ExampleObjectUpdate());

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Put_BadRequest_ShouldContainErrorResponse()
        {
            var result = await _controller.Put(999, new ExampleObjectUpdate()) as BadRequestObjectResult;

            result.Value.Should().BeOfType<ExampleAPI.Contracts.Shared.ErrorResponse>();
        }
    }
}
