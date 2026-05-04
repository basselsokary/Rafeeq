using Domain.Entities.ArtifactAggregate;
using Domain.Entities.SiteAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.Image;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Artifacts;

internal sealed class ArtifactConfiguration : IEntityTypeConfiguration<Artifact>
{
    public void Configure(EntityTypeBuilder<Artifact> builder)
    {
        builder.Property(a => a.MainImageUrl)
            .HasMaxLength(MaxImageUrlLength);

        builder.HasMany(a => a.Images)
            .WithOne()
            .HasForeignKey("ArtifactId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.LocalizedContents)
            .WithOne()
            .HasForeignKey("ArtifactId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Site>()
            .WithMany()
            .HasForeignKey(a => a.SiteId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.SiteId)
            .HasDatabaseName("IX_Artifacts_SiteId");
        
        builder.HasIndex(a => a.Type)
            .HasDatabaseName("IX_Artifacts_Type");
        
        builder.HasIndex(a => a.DisplayOrder)
            .HasDatabaseName("IX_Artifacts_DisplayOrder");
    }
}
