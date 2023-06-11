using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Topic.API.Models;

namespace Topic.API.Infrastructure.EntityConfigurations
{
    public class TopicTagEntityTypeConfiguration : IEntityTypeConfiguration<TopicTag>
    {
        public void Configure(EntityTypeBuilder<TopicTag> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id).IsRequired();
            builder.Property(c => c.Name).IsRequired().HasMaxLength(30);
        }
    }
}
