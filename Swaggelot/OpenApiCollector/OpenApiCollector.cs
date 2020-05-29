using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Swaggelot.Models;

namespace Swaggelot.OpenApiCollector
{
    public class OpenApiCollector : IOpenApiCollector
    {
        private readonly HttpClient _client;
        private readonly ILogger _logger;
        private readonly IEnumerable<SwaggerEndPointOptions> _swaggerEndpoints;

        public OpenApiCollector(
            IOptions<SwaggerSettings> swaggerSettings,
            HttpClient client,
            ILogger<OpenApiCollector> logger)
        {
            _client = client;
            _logger = logger;
            _swaggerEndpoints = swaggerSettings?.Value?.Endpoints ?? new List<SwaggerEndPointOptions>();
            if (swaggerSettings?.Value?.Endpoints == null)
            {
                _logger.LogWarning($"Swagger settings not found");
            }
        }

        public async Task<Dictionary<SwaggerDescriptor, OpenApiDocument>> CollectDownstreamSwaggersAsync()
        {
            var loaded = await Task.WhenAll(
                _swaggerEndpoints.SelectMany(
                    x => x.Versions,
                    (endpoint, config) => LoadSwaggerAsync(endpoint.Key, config)));

            return loaded.ToDictionary(x => x.Item1, x => x.Item2);
        }

        private async Task<(SwaggerDescriptor, OpenApiDocument)> LoadSwaggerAsync(string key,
            SwaggerEndPointConfig endpoint)
        {
            var descriptor = new SwaggerDescriptor(key, endpoint.Version);
            try
            {
                var swaggerJson = await _client.GetStringAsync(endpoint.Url);
                var openApi = new OpenApiStringReader().Read(swaggerJson, out _);
                return (descriptor, openApi);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, $"Can't load downstream swagger {endpoint.Url}");
                return (descriptor, null);
            }
        }
    }
}