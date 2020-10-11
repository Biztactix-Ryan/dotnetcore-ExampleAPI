using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ExampleAPI.Contracts.V2;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExampleAPI.Controllers.V2
{
    [ApiController]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Bloody Cold", "Somewhat Annoying", "Annoying", "Decent", "Boring", "Shirt Weather", "Tropical", "Alice Springs", "DesertHot", "Fry an Egg Hot"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(APIRoutes.Weather.Get)]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
