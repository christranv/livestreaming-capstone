using System;
using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Stream.Infrastructure;

namespace Stream.API.Infrastructure.Factories
{
    /**
     * Used to generate migration file
     */
    public class StreamingDbContextFactory : IDesignTimeDbContextFactory<StreamContext>
    {
        public StreamContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<StreamContext>();

            optionsBuilder.UseMySql(config["ConnectionString"], new MySqlServerVersion(new Version(8, 0, 21)),
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                    //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                    sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                });

            return new StreamContext(optionsBuilder.Options);
        }
    }
}