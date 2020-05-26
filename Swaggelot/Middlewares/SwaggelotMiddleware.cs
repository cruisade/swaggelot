using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Swaggelot.Cache;

namespace Swaggelot.Middlewares
{
    /// <summary>
    /// Middleware  for ocelot
    /// </summary>
    public class SwaggerForOcelotMiddleware
    {
        private readonly ISwaggerService _cache;

        public SwaggerForOcelotMiddleware(
            RequestDelegate next,
            ISwaggerService cache)
        {
            _cache = cache;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                var content = await _cache.GetSwagger();
                await context.Response.WriteAsync(content);
            }
            catch (Exception ex)
            { 
                //todo logger
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}