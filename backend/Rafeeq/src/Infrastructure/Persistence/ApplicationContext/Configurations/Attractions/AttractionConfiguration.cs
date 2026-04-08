using Domain.Entities.AttractionAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Attractions;

public sealed class AttractionConfiguration : IEntityTypeConfiguration<Attraction>
{
    public void Configure(EntityTypeBuilder<Attraction> builder)
    {
        builder.Property(a => a.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(a => a.MainImageUrl)
            .HasMaxLength(500);

        builder.Property(a => a.LocationDescription)
            .HasMaxLength(2000);

        builder.Property(a => a.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.HistoricalPeriod)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.IsFeatured)
            .HasDefaultValue(false);

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.LastModifiedAt);

        builder.OwnsOne(a => a.Location, location =>
        {
            location.Property(l => l.Latitude)
                .HasColumnName("Latitude")
                .HasPrecision(9, 6);

            location.Property(l => l.Longitude)
                .HasColumnName("Longitude")
                .HasPrecision(9, 6);
        });

        builder.Navigation(a => a.Location).IsRequired(false);

        builder.HasMany(a => a.Images)
            .WithOne()
            .HasForeignKey("AttractionId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.LocalizedContents)
            .WithOne()
            .HasForeignKey("AttractionId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.Name)
            .HasDatabaseName("IX_Attractions_Name");

        builder.HasIndex(a => a.Type)
            .HasDatabaseName("IX_Attractions_Type");

        builder.HasIndex(a => a.HistoricalPeriod)
            .HasDatabaseName("IX_Attractions_HistoricalPeriod");

        builder.HasIndex(a => a.IsFeatured)
            .HasDatabaseName("IX_Attractions_IsFeatured");

        builder.Ignore(a => a.DomainEvents);
    }
}
