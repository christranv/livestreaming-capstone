using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Topic.API.Models;

namespace Topic.API.Infrastructure.EntityConfigurations
{
    public class TopicCategoryTagsEntityTypeConfiguration : IEntityTypeConfiguration<TopicCategoryTags>
    {
        public void Configure(EntityTypeBuilder<TopicCategoryTags> builder)
        {
            builder.HasKey(k => new { k.CategoryId, k.TagId });
            builder.HasOne(pt => pt.Category).WithMany(t => t.CategoryTags).HasForeignKey(pt => pt.CategoryId);
            builder.HasOne(pt => pt.Tag).WithMany(t => t.CategoryTags).HasForeignKey(pt => pt.TagId);
        }
    }
}
