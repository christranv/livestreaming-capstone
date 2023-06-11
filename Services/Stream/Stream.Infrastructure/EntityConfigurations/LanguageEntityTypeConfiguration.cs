using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stream.Domain.AggregatesModel.StreamSessionAggregate;

namespace Stream.Infrastructure.EntityConfigurations
{
    class LanguageEntityTypeConfiguration
        : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> languageConfiguration)
        {
            languageConfiguration.ToTable("language");

            languageConfiguration.HasKey(o => o.Id);

            languageConfiguration.Property(o => o.Id)
                .ValueGeneratedNever()
                .IsRequired();

            languageConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
