using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stream.Domain.AggregatesModel.StreamSessionAggregate;

namespace Stream.Infrastructure.EntityConfigurations
{
    class StreamSessionTagEntityTypeConfiguration
        : IEntityTypeConfiguration<StreamSessionTag>
    {
        public void Configure(EntityTypeBuilder<StreamSessionTag> orderItemConfiguration)
        {
            orderItemConfiguration.ToTable("stream_session_tag");

            orderItemConfiguration.HasKey(o => o.Id);
            orderItemConfiguration.Ignore(b => b.DomainEvents);

            orderItemConfiguration.Property(t => t.TagGuid)
                .IsRequired();
            orderItemConfiguration
                .Property(t=>t.Name)
                .IsRequired();
        }
    }
}
