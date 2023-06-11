using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebStatus
{
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace;

        public static int Main(string[] args)
        {
            var configuration = GetConfiguration();

            //Log.Logđer = CreateSerilogLogger(configuration);

            try
            {
                //Log.Information("Configuring web host ({ApplicationContext})...", AppName);
                var host = BuildWebHost(configuration, args);

                LogPackagesVersionInfo();

                //Log.Information("Starting web host ({ApplicationContext})...", AppName);
                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                //Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
                Console.WriteLine(ex.ToString());
                return 1;
            }
            finally
            {
                //Log.CloseAndFlush();
            }
        }

        private static IWebHost BuildWebHost(IConfiguration configuration, string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseMetricsWebTracking()
                .UseMetrics(options =>
                {
                    options.EndpointOptions = endpointsOptions =>
                    {
                        endpointsOptions.MetricsTextEndpointOutputFormatter = new MetricsPrometheusTextOutputFormatter();
                        endpointsOptions.MetricsEndpointOutputFormatter = new MetricsPrometheusProtobufOutputFormatter();
                        endpointsOptions.EnvironmentInfoEndpointEnabled = false;
                    };
                })
                .CaptureStartupErrors(false)
                .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
                .UseStartup<Startup>()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .Build();

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }

        private static string GetVersion(Assembly assembly)
        {
            try
            {
                return $"{assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version} ({assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split()[0]})";
            }
            catch
            {
                return string.Empty;
            }
        }

        private static void LogPackagesVersionInfo()
        {
            var assemblies = new List<Assembly>();

            foreach (var dependencyName in typeof(Program).Assembly.GetReferencedAssemblies())
            {
                try
                {
                    // Try to load the referenced assembly...
                    assemblies.Add(Assembly.Load(dependencyName));
                }
                catch
                {
                    // Failed to load assembly. Skip it.
                }
            }

            var versionList = assemblies.Select(a => $"-{a.GetName().Name} - {GetVersion(a)}").OrderBy(value => value);

            //Log.Logger.ForContext("PackageVersions", string.Join("\n", versionList)).Information("Package versions ({ApplicationContext})", AppName);
        }
    }
}
