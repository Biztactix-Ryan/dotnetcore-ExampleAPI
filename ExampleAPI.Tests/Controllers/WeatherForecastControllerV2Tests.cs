using ExampleAPI.Controllers.V2;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ExampleAPI.Tests.Controllers
{
    public class WeatherForecastControllerV2Tests
    {
        private readonly WeatherForecastController _controller;

        public WeatherForecastControllerV2Tests()
        {
            var mockLogger = new Mock<ILogger<WeatherForecastController>>();
            _controller = new WeatherForecastController(mockLogger.Object);
        }

        [Fact]
        public void Get_ShouldReturn5Forecasts()
        {
            var result = _controller.Get();

            result.Should().HaveCount(5);
        }

        [Fact]
        public void Get_ShouldReturnFutureDates()
        {
            var result = _controller.Get().ToList();

            foreach (var forecast in result)
            {
                forecast.Date.Should().BeAfter(DateTime.Now);
            }
        }

        [Fact]
        public void Get_ShouldReturnTemperaturesInRange()
        {
            var result = _controller.Get().ToList();

            foreach (var forecast in result)
            {
                forecast.TemperatureC.Should().BeInRange(-20, 54);
            }
        }

        [Fact]
        public void Get_ShouldReturnNonNullSummaries()
        {
            var result = _controller.Get().ToList();

            foreach (var forecast in result)
            {
                forecast.Summary.Should().NotBeNullOrEmpty();
            }
        }

        [Fact]
        public void Get_ShouldReturnValidV2Summaries()
        {
            var validSummaries = new[]
            {
                "Bloody Cold", "Somewhat Annoying", "Annoying", "Decent", "Boring",
                "Shirt Weather", "Tropical", "Alice Springs", "DesertHot", "Fry an Egg Hot"
            };

            var result = _controller.Get().ToList();

            foreach (var forecast in result)
            {
                validSummaries.Should().Contain(forecast.Summary);
            }
        }

        [Fact]
        public void Get_ShouldReturnConsecutiveDays()
        {
            var result = _controller.Get().ToList();

            for (int i = 1; i < result.Count; i++)
            {
                result[i].Date.Should().BeAfter(result[i - 1].Date);
            }
        }
    }
}
