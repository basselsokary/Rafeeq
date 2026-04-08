using Domain.Entities.SponsorAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Sponsors;

public sealed class SponsorImageConfiguration : IEntityTypeConfiguration<SponsorImage>
{
    public void Configure(EntityTypeBuilder<SponsorImage> builder)
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

        builder.HasIndex("SponsorId", nameof(SponsorImage.IsMain))
            .HasDatabaseName("IX_SponsorImages_SponsorId_IsMain");

        builder.HasIndex("SponsorId", nameof(SponsorImage.DisplayOrder))
            .HasDatabaseName("IX_SponsorImages_SponsorId_DisplayOrder");

        builder.Ignore(i => i.DomainEvents);
    }
}
