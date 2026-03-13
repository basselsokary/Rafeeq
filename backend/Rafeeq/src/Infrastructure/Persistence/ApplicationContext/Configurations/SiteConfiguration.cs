using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RafeeqApp.Domain.Entities.SiteAggregate;

namespace RafeeqApp.Infrastructure.Persistence.ApplicationDbContext.Configurations;

public class SiteConfiguration : IEntityTypeConfiguration<Site>
{
    public void Configure(EntityTypeBuilder<Site> builder)
    {
        builder.ToTable("Sites");

        builder.HasKey(s => s.Id);

        // Properties
        builder.Property(s => s.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(s => s.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(s => s.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.Accessibility)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.EstimatedDurationMinutes)
            .IsRequired();

        builder.Property(s => s.AverageRating)
            .HasPrecision(3, 2);

        builder.Property(s => s.TotalReviews)
            .HasDefaultValue(0);

        builder.Property(s => s.TotalVisits)
            .HasDefaultValue(0);

        builder.Property(s => s.Website)
            .HasMaxLength(500);

        builder.Property(s => s.AudioGuideUrl)
            .HasMaxLength(500);

        builder.Property(s => s.VirtualTourUrl)
            .HasMaxLength(500);

        builder.Property(s => s.IsPopular)
            .HasDefaultValue(false);

        builder.Property(s => s.IsFeatured)
            .HasDefaultValue(false);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt);

        // Value Objects - Location
        builder.OwnsOne(s => s.Location, location =>
        {
            location.Property(l => l.Latitude)
                .HasColumnName("Latitude")
                .HasPrecision(9, 6)
                .IsRequired();

            location.Property(l => l.Longitude)
                .HasColumnName("Longitude")
                .HasPrecision(9, 6)
                .IsRequired();
        });

        // Value Objects - Address
        builder.OwnsOne(s => s.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("Street")
                .HasMaxLength(200)
                .IsRequired();

            address.Property(a => a.City)
                .HasColumnName("City")
                .HasMaxLength(100)
                .IsRequired();

            address.Property(a => a.Region)
                .HasColumnName("Region")
                .HasMaxLength(100);

            address.Property(a => a.Country)
                .HasColumnName("Country")
                .HasMaxLength(100)
                .IsRequired();

            address.Property(a => a.PostalCode)
                .HasColumnName("PostalCode")
                .HasMaxLength(20);
        });

        // Value Objects - EntryFee (Money)
        builder.OwnsOne(s => s.EntryFee, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("EntryFeeAmount")
                .HasPrecision(18, 2);

            money.Property(m => m.Currency)
                .HasColumnName("EntryFeeCurrency")
                .HasMaxLength(3);
        });

        // Value Objects - ContactPhone
        builder.OwnsOne(s => s.ContactPhone, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("ContactPhone")
                .HasMaxLength(20);
        });

        // Value Objects - ContactEmail
        builder.OwnsOne(s => s.ContactEmail, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("ContactEmail")
                .HasMaxLength(100);
        });

        // Relationships - Images (one-to-many)
        builder.HasMany(s => s.Images)
            .WithOne()
            .HasForeignKey("SiteId")
            .OnDelete(DeleteBehavior.Cascade);

        // Relationships - OpeningHours (one-to-many)
        builder.HasMany(s => s.OpeningHours)
            .WithOne()
            .HasForeignKey("SiteId")
            .OnDelete(DeleteBehavior.Cascade);

        // Relationships - LocalizedContents (one-to-many)
        builder.HasMany(s => s.LocalizedContents)
            .WithOne()
            .HasForeignKey("SiteId")
            .OnDelete(DeleteBehavior.Cascade);

        // Relationships - Facilities (one-to-many)
        builder.HasMany(s => s.Facilities)
            .WithOne()
            .HasForeignKey("SiteId")
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(s => s.Name)
            .HasDatabaseName("IX_Sites_Name");

        builder.HasIndex(s => s.Type)
            .HasDatabaseName("IX_Sites_Type");

        builder.HasIndex(s => s.Status)
            .HasDatabaseName("IX_Sites_Status");

        builder.HasIndex(s => new { s.Latitude, s.Longitude })
            .HasDatabaseName("IX_Sites_Location");

        builder.HasIndex(s => s.AverageRating)
            .HasDatabaseName("IX_Sites_AverageRating");

        builder.HasIndex(s => s.IsPopular)
            .HasDatabaseName("IX_Sites_IsPopular");

        builder.HasIndex(s => s.IsFeatured)
            .HasDatabaseName("IX_Sites_IsFeatured");

        // Ignore domain events (not persisted)
        builder.Ignore(s => s.DomainEvents);

        // Navigation properties metadata
        builder.Metadata
            .FindNavigation(nameof(Site.Images))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata
            .FindNavigation(nameof(Site.OpeningHours))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata
            .FindNavigation(nameof(Site.LocalizedContents))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata
            .FindNavigation(nameof(Site.Facilities))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
