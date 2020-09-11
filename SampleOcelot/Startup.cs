using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Swaggelot.Extensions;

namespace SampleOcelot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication()
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = Configuration["AUTHENTICATION_AUTHORITY"];
                    options.Audience = "api1";
                    options.RequireHttpsMetadata = false;
                })
                .AddJwtBearer("SomeOtherScheme", options =>
                {
                    options.Authority = Configuration["AUTHENTICATION_AUTHORITY"];
                    options.Audience = "api1";
                    options.RequireHttpsMetadata = false;
                });

            services.AddSwaggerForOcelot(Configuration);
            
            services.AddOcelot();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseSwaggerForOcelotUI();

            app.UseOcelot().Wait();
        }
    }
}