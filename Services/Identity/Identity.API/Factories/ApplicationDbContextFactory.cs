﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Identity.API.Data;
using Microsoft.Extensions.Configuration;
using System.IO;
using Identity.API.Infrastructure;

namespace Identity.API.Factories
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseMySql(config["ConnectionString"], new MySqlServerVersion(new Version(8, 0, 21)),
                mySqlOptionsAction: o => o.MigrationsAssembly("Identity.API"));

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}