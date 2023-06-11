using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Topic.API.Infrastructure;
using Topic.API.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Team5.BuildingBlocks.WebHost;

namespace Topic.API
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var configuration = GetConfiguration();

            try
            {
                var host = CreateHostBuilder(configuration, args);
                // ILogger logger = host.Services.GetService<ILogger<Program>>();
                // logger.LogInformation("Test");

                host.MigrateDbContext<TopicContext>((context, services) =>
                {
                    var env = services.GetService<IWebHostEnvironment>();
                    var settings = services.GetService<IOptions<TopicSettings>>();
                    var logger = services.GetService<ILogger<TopicContextSeed>>();

                    new TopicContextSeed()
                        .SeedAsync(context, env, settings, logger)
                        .Wait();
                });
                //.MigrateDbContext<IntegrationEventLogContext>((_, __) => { });

                host.Run();
                return 0;
            }
            catch (Exception)
            {
                return 1;
            }
        }

        private static IWebHost CreateHostBuilder(IConfiguration configuration, string[] args) =>
            WebHost.CreateDefaultBuilder(args).ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
                .CaptureStartupErrors(false)
                .ConfigureKestrel(options =>
                {
                    var ports = GetDefinedPorts(configuration);
                    options.Listen(IPAddress.Any, ports.httpPort,
                        listenOptions => { listenOptions.Protocols = HttpProtocols.Http1AndHttp2; });
                    options.Listen(IPAddress.Any, ports.grpcPort,
                        listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
                })
                .UseStartup<Startup>()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseWebRoot("wwwroot/category")
                .Build();

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            builder.Build();

            return builder.Build();
        }

        private static (int httpPort, int grpcPort) GetDefinedPorts(IConfiguration config)
        {
            var grpcPort = config.GetValue("GRPC_PORT", 81);
            var port = config.GetValue("PORT", 80);
            return (port, grpcPort);
        }
    }
}