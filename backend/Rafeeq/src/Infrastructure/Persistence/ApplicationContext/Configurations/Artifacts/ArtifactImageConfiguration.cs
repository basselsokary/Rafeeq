using Domain.Entities.ArtifactAggregate;
using Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.File;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Artifacts;

internal sealed class ArtifactImageConfiguration : IEntityTypeConfiguration<ArtifactImage>
{
    public void Configure(EntityTypeBuilder<ArtifactImage> builder)
    {
        builder.ToTable("ArtifactImages");

        builder.OwnsOne(i => i.StorageKey, key =>
        {
            key.Configure();
        });

        builder.Property(i => i.ImageUrl)
            .HasMaxLength(MaxImageUrlLength)
            .IsRequired();

        builder.Property(i => i.Caption)
            .HasMaxLength(MaxCaptionLength);

        builder.HasIndex("ArtifactId", nameof(ArtifactImage.IsMain))
            .HasDatabaseName("IX_ArtifactImages_ArtifactId_IsMain");

        builder.HasIndex("ArtifactId", nameof(ArtifactImage.DisplayOrder))
            .HasDatabaseName("IX_ArtifactImages_ArtifactId_DisplayOrder");
    }
}
