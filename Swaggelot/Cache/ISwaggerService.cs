using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Swaggelot.Models;
using Swaggelot.OpenApiCollector;

namespace Swaggelot.Cache
{
    public interface ISwaggerService
    {
        Task<string> GetSwagger();
    }

    public class SwaggerService : ISwaggerService
    {
        private readonly IOpenApiCollector _collector;
        private readonly IOptions<List<ReRouteOptions>> _reRoutes;
        private readonly IOptions<SwaggerSettings> _swaggerSettings;

        public SwaggerService(
            IOpenApiCollector collector,
            IOptions<List<ReRouteOptions>> reRoutes,
            IOptions<SwaggerSettings> swaggerSettings)
        {
            _collector = collector;
            _reRoutes = reRoutes;
            _swaggerSettings = swaggerSettings;
        }
        
        public Task<string> GetSwagger()
        {
            var transformer = new SwaggerTransformer(_collector, _reRoutes, _swaggerSettings);
            return transformer.Transform();
        }
    }
}