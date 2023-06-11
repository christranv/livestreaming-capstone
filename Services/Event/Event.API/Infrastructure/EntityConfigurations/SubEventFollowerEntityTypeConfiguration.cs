using Event.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Event.API.Infrastructure.EntityConfigurations
{
    public class SubEventFollowerEntityTypeConfiguration : IEntityTypeConfiguration<SubEventFollower>
    {
        public void Configure(EntityTypeBuilder<SubEventFollower> builder)
        {
            builder.ToTable("SubEventFollower");
            builder.HasKey(sef => sef.Id);

            builder.Property(sef => sef.UserId).IsRequired();
            builder.Property(sef => sef.CreateDate).IsRequired();

            builder.HasOne(sef => sef.SubEvent).WithMany(se => se.SubEventFollower);
        }   
    }
}