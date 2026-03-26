using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities.ReviewAggregate;

namespace Infrastructure.Persistence.ApplicationContext.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.Property(r => r.SiteId)
            .IsRequired();

        builder.Property(r => r.TouristId)
            .IsRequired();

        builder.Property(r => r.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(r => r.Content)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.HelpfulCount)
            .HasDefaultValue(0);

        builder.Property(r => r.NotHelpfulCount)
            .HasDefaultValue(0);

        builder.Property(r => r.RejectionReason)
            .HasMaxLength(500);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.LastModifiedAt);

        // Value Objects - Rating
        builder.OwnsOne(r => r.Rating, rating =>
        {
            rating.Property(rt => rt.Value)
                .HasColumnName("Rating")
                .IsRequired();

            rating.WithOwner();
        });

        // Indexes
        builder.HasIndex(r => r.SiteId)
            .HasDatabaseName("IX_Reviews_SiteId");

        builder.HasIndex(r => r.TouristId)
            .HasDatabaseName("IX_Reviews_TouristId");

        builder.HasIndex(r => r.Status)
            .HasDatabaseName("IX_Reviews_Status");

        builder.HasIndex(r => r.Rating)
            .HasDatabaseName("IX_Reviews_Rating");

        builder.HasIndex(r => r.CreatedAt)
            .HasDatabaseName("IX_Reviews_CreatedAt");

        builder.HasIndex(r => new { r.SiteId, r.Status })
            .HasDatabaseName("IX_Reviews_SiteId_Status");

        // Ignore domain events
        builder.Ignore(r => r.DomainEvents);
    }
}
