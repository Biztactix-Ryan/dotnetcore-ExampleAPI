using ExampleAPI.Contracts.Shared;
using ExampleAPI.Filters;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace ExampleAPI.Tests.Filters
{
    public class ValidationFilterTests
    {
        private readonly ValidationFilter _filter = new();

        private static ActionExecutingContext CreateContext(ModelStateDictionary modelState)
        {
            var httpContext = new DefaultHttpContext();
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor(), modelState);
            return new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new object());
        }

        [Fact]
        public async Task OnActionExecutionAsync_WhenModelStateValid_ShouldCallNext()
        {
            var context = CreateContext(new ModelStateDictionary());
            var nextCalled = false;
            ActionExecutionDelegate next = () =>
            {
                nextCalled = true;
                return Task.FromResult(new ActionExecutedContext(
                    new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
                    new List<IFilterMetadata>(),
                    new object()));
            };

            await _filter.OnActionExecutionAsync(context, next);

            nextCalled.Should().BeTrue();
            context.Result.Should().BeNull();
        }

        [Fact]
        public async Task OnActionExecutionAsync_WhenModelStateInvalid_ShouldReturnBadRequest()
        {
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("Name", "Name is required");
            var context = CreateContext(modelState);
            ActionExecutionDelegate next = () => throw new Exception("Should not be called");

            await _filter.OnActionExecutionAsync(context, next);

            context.Result.Should().BeOfType<BadRequestObjectResult>();
            var badRequest = context.Result as BadRequestObjectResult;
            var errorResponse = badRequest.Value as ErrorResponse;
            errorResponse.Errors.Should().HaveCount(1);
            errorResponse.Errors[0].FieldName.Should().Be("Name");
            errorResponse.Errors[0].Message.Should().Be("Name is required");
        }

        [Fact]
        public async Task OnActionExecutionAsync_WithMultipleErrors_ShouldReturnAllErrors()
        {
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("FirstName", "FirstName is required");
            modelState.AddModelError("LastName", "LastName is required");
            var context = CreateContext(modelState);
            ActionExecutionDelegate next = () => throw new Exception("Should not be called");

            await _filter.OnActionExecutionAsync(context, next);

            context.Result.Should().BeOfType<BadRequestObjectResult>();
            var badRequest = context.Result as BadRequestObjectResult;
            var errorResponse = badRequest.Value as ErrorResponse;
            errorResponse.Errors.Should().HaveCount(2);
        }

        [Fact]
        public async Task OnActionExecutionAsync_WithMultipleErrorsOnSameField_ShouldReturnAll()
        {
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("Email", "Email is required");
            modelState.AddModelError("Email", "Email format is invalid");
            var context = CreateContext(modelState);
            ActionExecutionDelegate next = () => throw new Exception("Should not be called");

            await _filter.OnActionExecutionAsync(context, next);

            context.Result.Should().BeOfType<BadRequestObjectResult>();
            var badRequest = context.Result as BadRequestObjectResult;
            var errorResponse = badRequest.Value as ErrorResponse;
            errorResponse.Errors.Should().HaveCount(2);
            errorResponse.Errors.Should().AllSatisfy(e => e.FieldName.Should().Be("Email"));
        }
    }
}
