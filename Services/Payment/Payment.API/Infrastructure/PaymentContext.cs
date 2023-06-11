using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Payment.API.Infrastructure.EntityConfigurations;
using Payment.API.Models;

namespace Payment.API.Infrastructure
{
    public class PaymentContext : DbContext
    {
        public PaymentContext(DbContextOptions<PaymentContext> options) : base(options)
        {
        }
        public DbSet<UserPayment> UserPayments { get; set; }
        public DbSet<DonateRecord> DonateRecords { get; set; }
        public DbSet<PaymentRecord> PaymentRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new DonateRecordEntityTypeConfiguration());
            builder.ApplyConfiguration(new PaymentRecordEntityTypeConfiguration());
            builder.ApplyConfiguration(new UserPaymentEntityTypeConfiguration());
        }
    }
    
    public class CatalogContextDesignFactory : IDesignTimeDbContextFactory<PaymentContext>
    {
        public PaymentContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PaymentContext>()
                .UseMySql("Server=sqldata;Database=team5_service_paymentdb;Uid=root;Pwd=Pass@word", new MySqlServerVersion(new Version(8, 0, 21)),
                    mySqlOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
    
            return new PaymentContext(optionsBuilder.Options);
        }
    }

}