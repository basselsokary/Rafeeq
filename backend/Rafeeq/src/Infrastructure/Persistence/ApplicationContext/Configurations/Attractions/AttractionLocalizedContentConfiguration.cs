using Domain.Entities.AttractionAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Attractions;

public sealed class AttractionLocalizedContentConfiguration : IEntityTypeConfiguration<AttractionLocalizedContent>
{
    public void Configure(EntityTypeBuilder<AttractionLocalizedContent> builder)
    {
        builder.Property(c => c.Language)
            .HasConversion<string>()
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(c => c.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.LastModifiedAt);

        builder.HasIndex("AttractionId", nameof(AttractionLocalizedContent.Language))
            .IsUnique()
            .HasDatabaseName("IX_AttractionLocalizedContents_AttractionId_Language");

        builder.Ignore(c => c.DomainEvents);
    }
}
