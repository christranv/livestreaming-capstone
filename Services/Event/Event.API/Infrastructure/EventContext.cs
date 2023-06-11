using System;
using System.Reflection;
using Event.API.Infrastructure.EntityConfigurations;
using Event.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage;

namespace Event.API.Infrastructure
{
    public class EventContext : DbContext
    {
        public EventContext(DbContextOptions<EventContext> options) : base(options)
        {
        }

        public DbSet<Models.Event> Events { get; set; }
        public DbSet<SubEvent> SubEvents { get; set; }
        public DbSet<SubEventFollower> SubEventFollowers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new EventEntityTypeConfiguration());
            builder.ApplyConfiguration(new SubEventEntityTypeConfiguration());
            builder.ApplyConfiguration(new SubEventFollowerEntityTypeConfiguration());
        }
    }

    public class CatalogContextDesignFactory : IDesignTimeDbContextFactory<EventContext>
    {
        public EventContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EventContext>()
                .UseMySql("Server=sqldata;Database=team5_service_eventdb;Uid=root;Pwd=Pass@word",
                    new MySqlServerVersion(new Version(8, 0, 21)),
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                        sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30),
                            null);
                    });

            return new EventContext(optionsBuilder.Options);
        }
    }
}