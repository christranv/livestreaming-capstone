using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payment.API.Models;

namespace Payment.API.Infrastructure.EntityConfigurations
{
    public class PaymentRecordEntityTypeConfiguration : IEntityTypeConfiguration<PaymentRecord>
    {
        public void Configure(EntityTypeBuilder<PaymentRecord> builder)
        {
            builder.ToTable("PaymentRecord");
            builder.HasKey(paymentRecord => paymentRecord.Id);

            builder.Property(paymentRecord => paymentRecord.TransactionType).HasConversion<int>().IsRequired();
            builder.Property(paymentRecord => paymentRecord.PaymentMethod).HasConversion<int>().IsRequired();
            builder.Property(paymentRecord => paymentRecord.Amount).IsRequired();
            builder.Property(paymentRecord => paymentRecord.UserId).IsRequired();
            builder.Property(paymentRecord => paymentRecord.CreateDate).IsRequired();
            
            builder.HasOne(s => s.UserPayment)
                .WithMany(g => g.PaymentHistory)
                .HasForeignKey(s => s.UserId);
        }
    }
}