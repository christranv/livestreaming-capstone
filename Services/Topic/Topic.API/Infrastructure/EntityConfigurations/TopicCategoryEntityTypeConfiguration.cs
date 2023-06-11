using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Topic.API.Models;

namespace Topic.API.Infrastructure.EntityConfigurations
{
    public class TopicCategoryEntityTypeConfiguration : IEntityTypeConfiguration<TopicCategory>
    {
        public void Configure(EntityTypeBuilder<TopicCategory> builder)
        {
            builder.ToTable("Category");
            builder.HasKey(c => c.Id);
            builder.HasIndex(c => c.DisplayOrder).IsUnique();
            builder.HasIndex(c => c.Name).IsUnique();

            builder.Property(c => c.Id).IsRequired();
            builder.Property(c => c.DisplayOrder).IsRequired();
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.PictureFilePath).IsRequired().HasMaxLength(150);
        }
    }
}
