using Domain.Entities.SiteAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Sites;

public sealed class SiteConfiguration : IEntityTypeConfiguration<Site>
{
    public void Configure(EntityTypeBuilder<Site> builder)
    {
        builder.Property(s => s.CityId)
            .IsRequired();

        builder.Property(s => s.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(s => s.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(s => s.ContactPhone)
            .HasMaxLength(20);

        builder.Property(s => s.WebsiteUrl)
            .HasMaxLength(500);

        builder.Property(s => s.MainImageUrl)
            .HasMaxLength(500);

        builder.Property(s => s.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.AverageRating)
            .HasPrecision(3, 2);

        builder.Property(s => s.TotalReviews)
            .HasDefaultValue(0);

        builder.Property(s => s.IsFeatured)
            .HasDefaultValue(false);

        builder.Property(s => s.IsActive)
            .HasDefaultValue(false);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.LastModifiedAt);

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

            address.Property(a => a.PostalCode)
                .HasColumnName("PostalCode")
                .HasMaxLength(20);
        });

        builder.OwnsOne(s => s.EntryFee, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("EntryFeeAmount")
                .HasPrecision(18, 2);

            money.Property(m => m.Currency)
                .HasColumnName("EntryFeeCurrency")
                .HasMaxLength(3);
        });

        builder.Navigation(s => s.EntryFee).IsRequired(false);

        builder.OwnsMany(s => s.OpeningHours, openingHours =>
        {
            openingHours.ToTable("SiteOpeningHours");
            openingHours.WithOwner().HasForeignKey("SiteId");

            openingHours.Property<int>("Id");
            openingHours.HasKey("Id");

            openingHours.Property(oh => oh.DayOfWeek)
                .HasConversion<string>()
                .HasMaxLength(16)
                .IsRequired();

            openingHours.Property(oh => oh.IsClosed)
                .IsRequired();

            openingHours.OwnsOne(oh => oh.OpeningTime, timeRange =>
            {
                timeRange.Property(t => t.StartTime)
                    .HasColumnName("StartTime")
                    .IsRequired();

                timeRange.Property(t => t.EndTime)
                    .HasColumnName("EndTime")
                    .IsRequired();
            });
        });

        builder.HasMany(s => s.Images)
            .WithOne()
            .HasForeignKey("SiteId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.LocalizedContents)
            .WithOne()
            .HasForeignKey("SiteId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Facilities)
            .WithOne()
            .HasForeignKey("SiteId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.NearestTransportations)
            .WithOne()
            .HasForeignKey("SiteId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => s.CityId)
            .HasDatabaseName("IX_Sites_CityId");

        builder.HasIndex(s => s.Name)
            .HasDatabaseName("IX_Sites_Name");

        builder.HasIndex(s => s.Type)
            .HasDatabaseName("IX_Sites_Type");

        builder.HasIndex(s => s.Status)
            .HasDatabaseName("IX_Sites_Status");

        builder.HasIndex(s => s.AverageRating)
            .HasDatabaseName("IX_Sites_AverageRating");

        builder.HasIndex(s => s.IsFeatured)
            .HasDatabaseName("IX_Sites_IsFeatured");

        builder.Ignore(s => s.DomainEvents);
    }
}
