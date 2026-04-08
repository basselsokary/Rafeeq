using Domain.Entities.TouristAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Tourists;

public sealed class TouristConfiguration : IEntityTypeConfiguration<Tourist>
{
    public void Configure(EntityTypeBuilder<Tourist> builder)
    {
        builder.Property(t => t.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.Nationality)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.PreferredLanguage)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.TotalTrips)
            .HasDefaultValue(0);

        builder.Property(t => t.TotalReviews)
            .HasDefaultValue(0);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.LastModifiedAt);

        builder.HasMany(t => t.Favourites)
            .WithOne()
            .HasForeignKey("TouristId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.Status)
            .HasDatabaseName("IX_Tourists_Status");

        builder.HasIndex(t => t.CreatedAt)
            .HasDatabaseName("IX_Tourists_CreatedAt");

        builder.Ignore(t => t.DomainEvents);
    }
}
