using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swaggelot.Models;
using Swaggelot.OpenApiCollector;

namespace Swaggelot.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSwaggerForOcelot(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<ISwaggerTransformer, SwaggerTransformer>();
            services
                .Configure<List<ReRouteOptions>>(options =>
                {
                    configuration.GetSection("ReRoutes").Bind(options);
                    configuration.GetSection("Routes").Bind(options);
                })
                .Configure<SwaggerSettings>(options
                    => configuration.GetSection(SwaggerSettings.ConfigurationSectionName).Bind(options))
                .AddHttpClient<IOpenApiCollector, OpenApiCollector.OpenApiCollector>();
            return services;
        }
    }
}