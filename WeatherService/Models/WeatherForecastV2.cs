namespace WeatherService
{
    public class WeatherForecastV2: WeatherForecastV1
    {
        public int TemperatureF => 32 + (int) (TemperatureC / 0.5556);
    }
}