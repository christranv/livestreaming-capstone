using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stream.Domain.AggregatesModel.StreamSessionAggregate;

namespace Stream.Infrastructure.EntityConfigurations
{
    class StreamSessionStatusEntityTypeConfiguration
        : IEntityTypeConfiguration<StreamSessionStatus>
    {
        public void Configure(EntityTypeBuilder<StreamSessionStatus> streamSessionStatusConfiguration)
        {
            streamSessionStatusConfiguration.ToTable("stream_session_status");

            streamSessionStatusConfiguration.HasKey(o => o.Id);

            streamSessionStatusConfiguration.Property(o => o.Id)
                .ValueGeneratedNever()
                .IsRequired();

            streamSessionStatusConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
