using Domain.Entities.CityAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Cities;

public sealed class CityLocalizedContentConfiguration : IEntityTypeConfiguration<CityLocalizedContent>
{
    public void Configure(EntityTypeBuilder<CityLocalizedContent> builder)
    {
        builder.Property(c => c.Language)
            .HasConversion<string>()
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.LastModifiedAt);

        builder.HasIndex("CityId", nameof(CityLocalizedContent.Language))
            .IsUnique()
            .HasDatabaseName("IX_CityLocalizedContents_CityId_Language");

        builder.Ignore(c => c.DomainEvents);
    }
}
