using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stream.Domain.AggregatesModel.StreamerAggregate;

namespace Stream.Infrastructure.EntityConfigurations
{
    class StreamerEntityTypeConfiguration : IEntityTypeConfiguration<Streamer>
    {
        public void Configure(EntityTypeBuilder<Streamer> streamerConfiguration)
        {
            streamerConfiguration.ToTable("streamer");

            streamerConfiguration.HasKey(s => s.Id);
            streamerConfiguration.Ignore(s => s.DomainEvents);

            streamerConfiguration.Property(s => s.IdentityGuid)
                .HasMaxLength(200)
                .IsRequired();
            streamerConfiguration.HasIndex(s => s.IdentityGuid)
                .IsUnique();
            
            streamerConfiguration.Property(s => s.AuthToken)
                .HasMaxLength(100)
                .IsRequired();
            
            streamerConfiguration.Property(s => s.StreamKey)
                .HasMaxLength(100)
                .IsRequired();
            
            streamerConfiguration.HasIndex(s => s.StreamKey)
                .IsUnique();
        }
    }
}