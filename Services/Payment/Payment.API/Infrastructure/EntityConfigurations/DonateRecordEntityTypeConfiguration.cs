using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payment.API.Models;

namespace Payment.API.Infrastructure.EntityConfigurations
{
    public class DonateRecordEntityTypeConfiguration : IEntityTypeConfiguration<DonateRecord>
    {
        public void Configure(EntityTypeBuilder<DonateRecord> builder)
        {
            builder.ToTable("DonateRecord");
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Amount).IsRequired();
            builder.Property(d => d.ReceiverIdentityGuid).IsRequired();
            builder.Property(d => d.StreamSessionId).IsRequired();
            builder.Property(d => d.UserName).IsRequired();
            builder.Property(d => d.Message).IsRequired();
            builder.Property(d => d.CreateDate).IsRequired();
            
            builder.HasOne(s => s.UserPayment)
                .WithMany(g => g.DonateHistory)
                .HasForeignKey(s => s.UserId);
        }
    }
}