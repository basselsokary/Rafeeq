using Domain.Entities.AttractionAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.Attraction;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Attractions;

internal sealed class AttractionLocalizedContentConfiguration : IEntityTypeConfiguration<AttractionLocalizedContent>
{
    public void Configure(EntityTypeBuilder<AttractionLocalizedContent> builder)
    {
        builder.Property(c => c.Name)
            .HasMaxLength(MaxNameLength)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(MaxDescriptionLength)
            .IsRequired();
        
        builder.Property(a => a.LocationDescription)
            .HasMaxLength(MaxLocationDescriptionLength)
            .IsRequired(false);

        builder.HasIndex("AttractionId", nameof(AttractionLocalizedContent.Language))
            .IsUnique()
            .HasDatabaseName("IX_AttractionLocalizedContents_AttractionId_Language");
        
        builder.HasIndex("AttractionId")
            .HasDatabaseName("IX_AttractionLocalizedContents_AttractionId");
            
        builder.HasIndex(a => a.Name)
            .HasDatabaseName("IX_AttractionLocalizedContents_Name");
    }
}
