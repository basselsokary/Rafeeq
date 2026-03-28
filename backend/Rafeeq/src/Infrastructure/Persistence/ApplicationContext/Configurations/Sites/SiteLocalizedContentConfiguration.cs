using Domain.Entities.SiteAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Sites;

public sealed class SiteLocalizedContentConfiguration : IEntityTypeConfiguration<SiteLocalizedContent>
{
    public void Configure(EntityTypeBuilder<SiteLocalizedContent> builder)
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

        builder.HasIndex("SiteId", nameof(SiteLocalizedContent.Language))
            .IsUnique()
            .HasDatabaseName("IX_SiteLocalizedContents_SiteId_Language");

        builder.Ignore(c => c.DomainEvents);
    }
}
