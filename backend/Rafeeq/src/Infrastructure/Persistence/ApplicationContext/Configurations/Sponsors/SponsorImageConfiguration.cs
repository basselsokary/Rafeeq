using Domain.Entities.SponsorAggregate;
using Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.Image;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Sponsors;

internal sealed class SponsorImageConfiguration : IEntityTypeConfiguration<SponsorImage>
{
    public void Configure(EntityTypeBuilder<SponsorImage> builder)
    {
        builder.ToTable("SponsorImages");

        builder.OwnsOne(i => i.StorageKey, key =>
        {
            key.Configure();
        });
        
        builder.Property(i => i.ImageUrl)
            .HasMaxLength(MaxImageUrlLength)
            .IsRequired();

        builder.Property(i => i.Caption)
            .HasMaxLength(MaxCaptionLength);

        builder.HasIndex("SponsorId", nameof(SponsorImage.IsMain))
            .HasDatabaseName("IX_SponsorImages_SponsorId_IsMain");

        builder.HasIndex("SponsorId", nameof(SponsorImage.DisplayOrder))
            .HasDatabaseName("IX_SponsorImages_SponsorId_DisplayOrder");
    }
}
