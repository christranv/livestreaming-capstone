using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Topic.API.Infrastructure.EntityConfigurations;
using Topic.API.Models;

namespace Topic.API.Infrastructure
{
    public class TopicContext : DbContext
    {
        public TopicContext(DbContextOptions<TopicContext> options) : base(options)
        {
        }
        public DbSet<TopicCategory> CategoryItems { get; set; }
        public DbSet<TopicTag> TagItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new TopicCategoryEntityTypeConfiguration());
            builder.ApplyConfiguration(new TopicTagEntityTypeConfiguration());
            builder.ApplyConfiguration(new TopicCategoryTagsEntityTypeConfiguration());
        }
    }

    public class CatalogContextDesignFactory : IDesignTimeDbContextFactory<TopicContext>
    {
        public TopicContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TopicContext>()
            .UseMySql("Server=sqldata;Database=team5_service_topicdb;Uid=root;Pwd=Pass@word", new MySqlServerVersion(new Version(8, 0, 21)),
                     mySqlOptionsAction: sqlOptions =>
                     {
                         sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                         sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                     });

            return new TopicContext(optionsBuilder.Options);
        }
    }
}

    
