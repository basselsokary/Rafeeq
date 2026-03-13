using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RafeeqApp.Domain.Entities.CityAggregate;

namespace RafeeqApp.Infrastructure.Persistence.ApplicationDbContext.Configurations;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("Cities");

        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(2000);

        builder.Property(c => c.Region)
            .HasMaxLength(100);

        builder.Property(c => c.Country)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Population);

        builder.Property(c => c.IsPopular)
            .HasDefaultValue(false);

        builder.Property(c => c.ImageUrl)
            .HasMaxLength(500);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt);

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

        builder.HasIndex(c => c.Country)
            .HasDatabaseName("IX_Cities_Country");

        builder.HasIndex(c => c.IsPopular)
            .HasDatabaseName("IX_Cities_IsPopular");

        // Ignore domain events
        builder.Ignore(c => c.DomainEvents);
    }
}
