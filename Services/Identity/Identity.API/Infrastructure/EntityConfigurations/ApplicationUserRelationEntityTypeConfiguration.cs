using Identity.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.API.Infrastructure.EntityConfigurations
{
    public class ApplicationUserRelationEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUserRelation>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserRelation> builder)
        {
            builder.ToTable("ApplicationUserRelation");
            builder.HasKey(k => new { k.UserAId, k.UserBId });
            builder.HasOne(pt => pt.UserA).WithMany(t => t.FollowingUsers).HasForeignKey(pt => pt.UserAId);
            builder.HasOne(pt => pt.UserB).WithMany(t => t.FollowedByUsers).HasForeignKey(pt => pt.UserBId);
        }
    }
}
