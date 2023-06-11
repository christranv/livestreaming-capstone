using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Team5.BuildingBlocks.Core.Infrastructure.Filters;
using Team5.BuildingBlocks.Core.Swagger;

namespace Team5.BuildingBlocks.Core
{
    /**
     * Notice: For Api only
     * Include HealthCheck and Swagger
     */
    public static class CoreExtension
    {
        public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCustomSwagger(configuration);

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .SetIsOriginAllowed(_ => true)
                        .AllowCredentials()
                );
            });
            // Must add controller last to apply all config
            services.AddControllers(options => { options.Filters.Add(typeof(HttpGlobalExceptionFilter)); })
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                );

            return services;
        }

        public static void UseCore(this IApplicationBuilder app, bool isDevelopment = true,
            string pathBase = null)
        {
            if (isDevelopment)
            {
                app.UseCustomSwagger(pathBase);
            }

            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();
            // Must be last to apply all config
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
            });
        }
    }
}