using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Team5.BuildingBlocks.MessageBroker.IntegrationEventLogEF;

namespace Payment.API.Infrastructure.IntegrationEventMigrations
{
    /**
     * Used to generate migration file
     */
    public class IntegrationEventLogContextDesignTimeFactory : IDesignTimeDbContextFactory<IntegrationEventLogContext>
    {
        public IntegrationEventLogContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
            
            var optionsBuilder = new DbContextOptionsBuilder<IntegrationEventLogContext>();

            optionsBuilder.UseMySql(config["ConnectionString"], new MySqlServerVersion(new Version(8, 0, 21)),
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(GetType().Assembly.GetName().Name);
                    //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                    sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                });

            return new IntegrationEventLogContext(optionsBuilder.Options);
        }
    }
}