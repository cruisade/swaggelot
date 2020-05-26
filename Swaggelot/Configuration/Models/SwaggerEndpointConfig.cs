namespace Swaggelot.Models
{
    public class SwaggerEndPointConfig
    {
        public int Version { get; set; }

        /// <summary>
        /// Full url to downstream service swagger endpoint.
        /// </summary>
        /// <example>http://localhost:5100/swagger/v1/swagger.json</example>
        public string Url { get; set; }
    }
}