using Domain.Entities.SiteAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Sites;

public sealed class SiteImageConfiguration : IEntityTypeConfiguration<SiteImage>
{
    public void Configure(EntityTypeBuilder<SiteImage> builder)
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

        builder.HasIndex("SiteId", nameof(SiteImage.IsMain))
            .HasDatabaseName("IX_SiteImages_SiteId_IsMain");

        builder.HasIndex("SiteId", nameof(SiteImage.DisplayOrder))
            .HasDatabaseName("IX_SiteImages_SiteId_DisplayOrder");

        builder.Ignore(i => i.DomainEvents);
    }
}
