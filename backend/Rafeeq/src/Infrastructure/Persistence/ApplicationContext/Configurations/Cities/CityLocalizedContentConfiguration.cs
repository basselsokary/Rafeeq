using Domain.Entities.CityAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.City;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Cities;

internal sealed class CityLocalizedContentConfiguration : IEntityTypeConfiguration<CityLocalizedContent>
{
    public void Configure(EntityTypeBuilder<CityLocalizedContent> builder)
    {
        builder.Property(c => c.Name)
            .HasMaxLength(MaxNameLength)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(MaxDescriptionLength)
            .IsRequired();

        builder.HasIndex(c => c.Name)
            .IsUnique()
            .HasDatabaseName("IX_CityLocalizedContents_Name");

        builder.HasIndex("CityId", nameof(CityLocalizedContent.Language))
            .IsUnique()
            .HasDatabaseName("IX_CityLocalizedContents_CityId_Language");
        
        builder.HasIndex("CityId")
            .HasDatabaseName("IX_CityLocalizedContents_CityId");
    }
}
