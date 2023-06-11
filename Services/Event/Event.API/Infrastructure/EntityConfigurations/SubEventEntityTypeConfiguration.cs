using Event.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Event.API.Infrastructure.EntityConfigurations
{
    public class SubEventEntityTypeConfiguration : IEntityTypeConfiguration<SubEvent>
    {
        public void Configure(EntityTypeBuilder<SubEvent> builder)
        {
            builder.ToTable("SubEvent");
            builder.HasKey(se => se.Id);
            
            builder.Property(se => se.Status).IsRequired();
            builder.Property(se => se.StartTime).IsRequired();
            builder.Property(se => se.StreamSessionId);
            
            builder.HasOne(e => e.Event).WithMany(se=>se.SubEvents);
        }
    }
}