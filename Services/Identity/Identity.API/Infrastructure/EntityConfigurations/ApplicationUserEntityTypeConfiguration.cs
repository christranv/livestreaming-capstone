using Identity.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.API.Infrastructure.EntityConfigurations
{
    public class ApplicationUserEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("ApplicationUser");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id).IsRequired();
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Gender).HasDefaultValue(0);
            builder.Property(c => c.Biography).HasDefaultValue("").HasMaxLength(150);
            builder.Property(c => c.ProfileImage).HasDefaultValue("").HasMaxLength(150);
            builder.Property(c => c.BannerImage).HasDefaultValue("").HasMaxLength(150);
        }
    }
}
