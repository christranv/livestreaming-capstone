using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Stream.SignalrHub.Workers;
using Team5.BuildingBlocks.Logging.Seriloger;

namespace Stream.SignalrHub
{
    public class Program
    {
        private static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace;

        public static int Main(string[] args)
        {
            var configuration = GetConfiguration();

            Log.Logger = SerilogConfig.CreateSerilogLogger(configuration, AppName);

            try
            {
                Log.Information("Configuring web host ({ApplicationContext})...", AppName);
                var host = BuildWebHost(configuration, args);

                Log.Information("Starting web host ({ApplicationContext})...", AppName);
                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IWebHost BuildWebHost(IConfiguration configuration, string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .CaptureStartupErrors(false)
                .ConfigureKestrel(options =>
                {
                    var port = GetDefinedPort(configuration);
                    options.Listen(IPAddress.Any, port,
                        listenOptions => { listenOptions.Protocols = HttpProtocols.Http1AndHttp2; });
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<StreamCountUpdateWorker>();
                })
                .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
                .UseStartup<Startup>()
                .UseSerilog()
                .Build();
        
        private static int GetDefinedPort(IConfiguration config)
        {
            var port = config.GetValue("PORT", 80);
            return port;
        }

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}