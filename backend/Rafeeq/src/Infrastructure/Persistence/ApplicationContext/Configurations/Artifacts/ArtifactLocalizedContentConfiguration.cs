using Domain.Entities.ArtifactAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.Artifact;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Artifacts;

internal sealed class ArtifactLocalizedContentConfiguration : IEntityTypeConfiguration<ArtifactLocalizedContent>
{
    public void Configure(EntityTypeBuilder<ArtifactLocalizedContent> builder)
    {
        builder.Property(c => c.Name)
            .HasMaxLength(MaxNameLength)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(MaxDescriptionLength)
            .IsRequired();

        builder.HasIndex("ArtifactId", nameof(ArtifactLocalizedContent.Language))
            .IsUnique()
            .HasDatabaseName("IX_ArtifactLocalizedContents_ArtifactId_Language");
        
        builder.HasIndex(a => a.Name)
            .HasDatabaseName("IX_ArtifactLocalizedContents_Name");
    }
}
