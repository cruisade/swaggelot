using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Swaggelot.Middlewares
{
    /// <summary>
    /// Middleware  for ocelot
    /// </summary>
    public class SwaggerForOcelotMiddleware
    {
        public SwaggerForOcelotMiddleware(RequestDelegate next)
        {
        }

        public async Task InvokeAsync(
            HttpContext context,
            ISwaggerTransformer transformer,
            ILogger<SwaggerForOcelotMiddleware> logger)
        {
            try
            {
                var content = await transformer.Transform();
                await context.Response.WriteAsync(content);
            }
            catch (Exception ex)
            {
                //logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}