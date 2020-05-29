using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Swaggelot.Middlewares
{
    /// <summary>
    /// Middleware  for ocelot
    /// </summary>
    public class SwaggelotMiddleware
    {
        public SwaggelotMiddleware(RequestDelegate next)
        {
        }

        public async Task InvokeAsync(
            HttpContext context,
            ISwaggerTransformer transformer,
            ILogger<SwaggelotMiddleware> logger)
        {
            try
            {
                var content = await transformer.Transform();
                await context.Response.WriteAsync(content);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}