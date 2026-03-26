using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities.TouristAggregate;

namespace Infrastructure.Persistence.ApplicationContext.Configurations;

public class TouristConfiguration : IEntityTypeConfiguration<Tourist>
{
    public void Configure(EntityTypeBuilder<Tourist> builder)
    {
        builder.Property(u => u.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.PreferredLanguage)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.Nationality)
            .HasMaxLength(100);

        builder.Property(u => u.TotalTrips)
            .HasDefaultValue(0);

        builder.Property(u => u.TotalReviews)
            .HasDefaultValue(0);

        builder.HasMany(u => u.Favourites)
            .WithOne()
            .HasForeignKey("TouristId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(u => u.Status)
            .HasDatabaseName("IX_Tourists_Status");

        builder.HasIndex(u => u.CreatedAt)
            .HasDatabaseName("IX_Tourists_CreatedAt");

        // Ignore domain events
        builder.Ignore(u => u.DomainEvents);
    }
}
