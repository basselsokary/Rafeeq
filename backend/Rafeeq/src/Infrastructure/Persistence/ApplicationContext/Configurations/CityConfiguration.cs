using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities.CityAggregate;

namespace Infrastructure.Persistence.ApplicationContext.Configurations;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(2000);

        builder.Property(c => c.ImageUrl)
            .HasMaxLength(500);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.LastModifiedAt);

        // Value Objects - Location (city center)
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

        // Indexes
        builder.HasIndex(c => c.Name)
            .HasDatabaseName("IX_Cities_Name");

        // Ignore domain events
        builder.Ignore(c => c.DomainEvents);
    }
}
