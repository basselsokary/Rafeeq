using Domain.Entities;
using Domain.Entities.AttractionAggregate;
using Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.File;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Attractions;

internal sealed class AttractionImageConfiguration : IEntityTypeConfiguration<AttractionImage>
{
    public void Configure(EntityTypeBuilder<AttractionImage> builder)
    {
        builder.ToTable("AttractionImages");

        builder.OwnsOne(i => i.StorageKey, key =>
        {
            key.Configure();
        });

        builder.Property(i => i.ImageUrl)
            .HasMaxLength(MaxImageUrlLength)
            .IsRequired();

        builder.Property(i => i.Caption)
            .HasMaxLength(MaxCaptionLength);

        builder.HasOne<StoredFile>()
            .WithMany()
            .HasForeignKey(i => i.StoredFileId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex("AttractionId", nameof(AttractionImage.IsMain))
            .HasDatabaseName("IX_AttractionImages_AttractionId_IsMain");

        builder.HasIndex("AttractionId", nameof(AttractionImage.DisplayOrder))
            .HasDatabaseName("IX_AttractionImages_AttractionId_DisplayOrder");
    }
}
