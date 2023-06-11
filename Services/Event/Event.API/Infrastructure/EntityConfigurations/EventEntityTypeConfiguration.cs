using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Event.API.Infrastructure.EntityConfigurations
{
    public class EventEntityTypeConfiguration : IEntityTypeConfiguration<Models.Event>
    {
        public void Configure(EntityTypeBuilder<Models.Event> builder)
        {
            builder.ToTable("Event");
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.Name).IsRequired();
            builder.Property(e => e.CategoryId).IsRequired();
            builder.Property(e => e.StartTime).IsRequired();
            builder.Property(e => e.EndTime).IsRequired();
            builder.Property(e => e.LogoImageFilePath).IsRequired();

            builder.HasMany(e => e.SubEvents).WithOne(se=>se.Event);
        }
    }
}