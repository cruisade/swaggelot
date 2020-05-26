using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WeatherService.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/weatherforecast")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get weather forecast
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<WeatherForecastV1> GetV1([FromQuery] string city)
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecastV1
                {
                    City = string.IsNullOrEmpty(city) ? "Default city" : city,
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
        }
        
        /// <summary>
        /// Get weather forecast
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiVersion("2.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<WeatherForecastV1> GetV2([FromQuery] string city)
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecastV2
                {
                    City = string.IsNullOrEmpty(city) ? "Default city" : city,
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
        }
        
    }
}