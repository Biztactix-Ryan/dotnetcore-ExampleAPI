using ExampleAPI.Models;
using FluentAssertions;
using Xunit;

namespace ExampleAPI.Tests.Models
{
    public class WeatherForecastTests
    {
        [Theory]
        [InlineData(0, 32)]
        [InlineData(100, 212)]
        [InlineData(25, 77)]
        [InlineData(-20, -4)]
        [InlineData(-40, -40)]
        public void TemperatureF_ShouldConvertFromCelsius(int tempC, int expectedF)
        {
            var forecast = new WeatherForecast { TemperatureC = tempC };

            forecast.TemperatureF.Should().Be(expectedF);
        }

        [Fact]
        public void Properties_ShouldBeSettable()
        {
            var date = new DateTime(2024, 1, 15);
            var forecast = new WeatherForecast
            {
                Date = date,
                TemperatureC = 20,
                Summary = "Warm"
            };

            forecast.Date.Should().Be(date);
            forecast.TemperatureC.Should().Be(20);
            forecast.Summary.Should().Be("Warm");
        }
    }
}
