using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Team5.BuildingBlocks.Core.Swagger
{
    public static class SwaggerExtension
    {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services,
            IConfiguration configuration)
        {
            var options = new SwaggerOptions();

            if (string.IsNullOrWhiteSpace(options.Title))
            {
                options.Title = AppDomain.CurrentDomain.FriendlyName.Trim().Trim('_');
            }

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(options.VersionName, options);
                c.CustomSchemaIds(x => x.FullName);
            });

            return services;
        }

        public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app, string pathBase = null)
        {
            var options = new SwaggerOptions();

            if (string.IsNullOrWhiteSpace(options.Title))
            {
                options.Title = AppDomain.CurrentDomain.FriendlyName.Trim().Trim('_');
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"{(pathBase ?? string.Empty)}/swagger/{options.VersionName}/swagger.json",
                    options.Title);
                c.RoutePrefix = options.RoutePrefix;
            });

            return app;
        }
    }
}