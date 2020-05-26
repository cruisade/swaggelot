using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Swaggelot.Middlewares;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Swaggelot.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSwaggerForOcelotUI(
            this IApplicationBuilder app,
            Action<SwaggerUIOptions> setupAction = null)
        {
            UseSwaggerForOcelot(app);

            app.UseSwaggerUI(c =>
            {
                setupAction?.Invoke(c);
                c.SwaggerEndpoint("/v1/swagger.json", "Awesome Ocelot");
            });

            return app;
        }

        private static void UseSwaggerForOcelot(IApplicationBuilder app)
            => app.Map("/v1/swagger.json", builder =>
                builder.UseMiddleware<SwaggerForOcelotMiddleware>());
    }
}