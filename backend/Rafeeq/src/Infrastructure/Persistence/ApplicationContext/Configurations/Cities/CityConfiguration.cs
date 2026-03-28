using Domain.Entities.CityAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Cities;

public sealed class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(c => c.ImageUrl)
            .HasMaxLength(500);

        builder.Property(c => c.TotalSites)
            .HasDefaultValue(0);

        builder.Property(c => c.DisplayOrder)
            .HasDefaultValue(0);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.LastModifiedAt);

        builder.OwnsOne(c => c.CenterLocation, location =>
        {
            location.Property(l => l.Latitude)
                .HasColumnName("CenterLatitude")
                .HasPrecision(9, 6)
                .IsRequired();

            location.Property(l => l.Longitude)
                .HasColumnName("CenterLongitude")
                .HasPrecision(9, 6)
                .IsRequired();
        });

        builder.HasMany(c => c.LocalizedContents)
            .WithOne()
            .HasForeignKey("CityId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.Name)
            .HasDatabaseName("IX_Cities_Name");

        builder.HasIndex(c => c.DisplayOrder)
            .HasDatabaseName("IX_Cities_DisplayOrder");

        builder.Ignore(c => c.DomainEvents);
    }
}
