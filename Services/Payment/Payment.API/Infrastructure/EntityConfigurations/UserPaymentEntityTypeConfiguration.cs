using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payment.API.Models;

namespace Payment.API.Infrastructure.EntityConfigurations
{
    public class UserPaymentEntityTypeConfiguration : IEntityTypeConfiguration<UserPayment>
    {
        public void Configure(EntityTypeBuilder<UserPayment> builder)
        {
            builder.ToTable("UserPayment");
            builder.HasKey(userPayment => userPayment.UserId);

            builder.Property(userPayment => userPayment.Balance).IsRequired();
            builder.Property(userPayment => userPayment.CreatedDate).IsRequired();
        }
    }
}