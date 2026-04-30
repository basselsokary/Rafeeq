using Domain.Entities.CityAggregate;
using Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.Image;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Cities;

internal sealed class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.Property(c => c.ImageUrl)
            .HasMaxLength(MaxImageUrlLength);

        builder.OwnsOne(i => i.StorageKey, key =>
        {
            key.Configure();
        });

        builder.OwnsOne(c => c.CenterLocation, location => 
        {
            location.Configure();
        });

        builder.HasMany(c => c.LocalizedContents)
            .WithOne()
            .HasForeignKey("CityId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.DisplayOrder)
            .HasDatabaseName("IX_Cities_DisplayOrder");
    }
}
