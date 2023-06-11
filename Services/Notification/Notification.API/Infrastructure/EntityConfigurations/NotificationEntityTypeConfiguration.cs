using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Notification.API.Infrastructure.EntityConfigurations
{
    public class NotificationEntityTypeConfiguration : IEntityTypeConfiguration<Models.Notification>
    {
        public void Configure(EntityTypeBuilder<Models.Notification> builder)
        {
            builder.ToTable("Notification");
            builder.HasKey(n => n.Id);
            builder.Property(n => n.Id).ValueGeneratedOnAdd();
            builder.Property(e => e.Type).HasConversion<int>().IsRequired();
            builder.Property(n => n.ActionName).HasConversion<int>().IsRequired();
            builder.Property(n => n.UserId).IsRequired();
            builder.Property(n => n.Content).IsRequired();
        }
    }
}