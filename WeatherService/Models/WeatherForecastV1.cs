using System;

namespace WeatherService
{
    public class WeatherForecastV1
    {
        public string City { get; set; }

        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public string Summary { get; set; }
    }
}