using Domain.Entities.AttractionAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Attractions;

public sealed class AttractionImageConfiguration : IEntityTypeConfiguration<AttractionImage>
{
    public void Configure(EntityTypeBuilder<AttractionImage> builder)
    {
        builder.Property(i => i.ImageUrl)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(i => i.Caption)
            .HasMaxLength(500);

        builder.Property(i => i.DisplayOrder)
            .IsRequired();

        builder.Property(i => i.CreatedAt)
            .IsRequired();

        builder.Property(i => i.LastModifiedAt);

        builder.HasIndex("AttractionId", nameof(AttractionImage.IsMain))
            .HasDatabaseName("IX_AttractionImages_AttractionId_IsMain");

        builder.HasIndex("AttractionId", nameof(AttractionImage.DisplayOrder))
            .HasDatabaseName("IX_AttractionImages_AttractionId_DisplayOrder");

        builder.Ignore(i => i.DomainEvents);
    }
}
