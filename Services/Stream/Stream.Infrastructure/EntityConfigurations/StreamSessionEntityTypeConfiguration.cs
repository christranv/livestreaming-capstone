using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stream.Domain.AggregatesModel.StreamerAggregate;
using Stream.Domain.AggregatesModel.StreamSessionAggregate;

namespace Stream.Infrastructure.EntityConfigurations
{
    class StreamSessionEntityTypeConfiguration : IEntityTypeConfiguration<StreamSession>
    {
        public void Configure(EntityTypeBuilder<StreamSession> streamSessionConfiguration)
        {
            streamSessionConfiguration.ToTable("stream_session");

            streamSessionConfiguration.HasKey(o => o.Id);
            streamSessionConfiguration.Ignore(b => b.DomainEvents);

            streamSessionConfiguration
                .Property(ss => ss.StreamerId)
                .IsRequired();
            streamSessionConfiguration
                .Property(ss => ss.StreamerName);
            streamSessionConfiguration
                .Property(ss => ss.StreamerImageUrl);
            streamSessionConfiguration
                .Property(ss => ss.ViewCount)
                .IsRequired();
            streamSessionConfiguration
                .Property(ss => ss.CreatedDate)
                .IsRequired();
            streamSessionConfiguration
                .Property(ss => ss.Title)
                .HasMaxLength(200);
            streamSessionConfiguration
                .Property(ss => ss.Announcement)
                .HasMaxLength(200);

            // Config streamSessionStatus
            streamSessionConfiguration
                .Property(ss => ss.StreamSessionStatusId)
                .IsRequired();
            streamSessionConfiguration.HasOne(p => p.StreamSessionStatus)
                .WithMany()
                .HasForeignKey(ss => ss.StreamSessionStatusId);

            // Config languageId
            streamSessionConfiguration
                .Property(ss => ss.LanguageId)
                .IsRequired();
            streamSessionConfiguration.HasOne(p => p.Language)
                .WithMany()
                .HasForeignKey(ss => ss.LanguageId);

            streamSessionConfiguration
                .Property(ss => ss.SubEventId);
            streamSessionConfiguration.HasIndex(ss => ss.SubEventId)
                .IsUnique();
            streamSessionConfiguration
                .Property(ss => ss.ThumbnailImage);

            streamSessionConfiguration.HasOne<StreamSessionStatus>()
                .WithMany()
                .HasForeignKey(ss => ss.StreamSessionStatusId)
                .IsRequired();

            // Relations configs
            streamSessionConfiguration.HasOne(ss => ss.Streamer)
                .WithMany()
                .HasForeignKey(ss => ss.StreamerId)
                .IsRequired();

            streamSessionConfiguration.HasOne(ss => ss.Category)
                .WithMany()
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // DDD Patterns comment:
            //Set as field (New since EF 1.1) to access the Tag collection property through its field
            var navigation = streamSessionConfiguration.Metadata.FindNavigation(nameof(StreamSession.Tags));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}