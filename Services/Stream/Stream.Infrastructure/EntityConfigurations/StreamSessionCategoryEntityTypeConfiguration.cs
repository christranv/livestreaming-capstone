using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stream.Domain.AggregatesModel.StreamSessionAggregate;

namespace Stream.Infrastructure.EntityConfigurations
{
    class StreamSessionCategoryEntityTypeConfiguration
        : IEntityTypeConfiguration<StreamSessionCategory>
    {
        public void Configure(EntityTypeBuilder<StreamSessionCategory> categoryConfiguration)
        {
            categoryConfiguration.ToTable("stream_session_category");

            categoryConfiguration.HasKey(o => o.Id);
            categoryConfiguration.Ignore(b => b.DomainEvents);

            categoryConfiguration.Property(c => c.CategoryGuid)
                .HasMaxLength(36)
                .IsRequired();
            categoryConfiguration.Property(c => c.Name)
                .IsRequired();
        }
    }
}
