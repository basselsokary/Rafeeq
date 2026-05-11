using Domain.Entities;
using Domain.Entities.SiteAggregate;
using Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.File;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Sites;

internal sealed class SiteImageConfiguration : IEntityTypeConfiguration<SiteImage>
{
    public void Configure(EntityTypeBuilder<SiteImage> builder)
    {
        builder.ToTable("SiteImages");

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

        builder.HasIndex("SiteId", nameof(SiteImage.IsMain))
            .HasDatabaseName("IX_SiteImages_SiteId_IsMain");

        builder.HasIndex("SiteId", nameof(SiteImage.DisplayOrder))
            .HasDatabaseName("IX_SiteImages_SiteId_DisplayOrder");
    }
}
