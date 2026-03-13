using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RafeeqApp.Domain.Entities.ReviewAggregate;

namespace RafeeqApp.Infrastructure.Persistence.ApplicationDbContext.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");

        builder.HasKey(r => r.Id);

        // Properties
        builder.Property(r => r.SiteId)
            .IsRequired();

        builder.Property(r => r.UserId)
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

        builder.Property(r => r.VisitDate)
            .IsRequired();

        builder.Property(r => r.HelpfulCount)
            .HasDefaultValue(0);

        builder.Property(r => r.NotHelpfulCount)
            .HasDefaultValue(0);

        builder.Property(r => r.IsVerifiedVisit)
            .HasDefaultValue(false);

        builder.Property(r => r.RejectionReason)
            .HasMaxLength(500);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.UpdatedAt);

        // Value Objects - Rating
        builder.OwnsOne(r => r.Rating, rating =>
        {
            rating.Property(rt => rt.Value)
                .HasColumnName("Rating")
                .IsRequired();

            rating.WithOwner();
        });

        // Relationships - Images (one-to-many)
        builder.HasMany(r => r.Images)
            .WithOne()
            .HasForeignKey("ReviewId")
            .OnDelete(DeleteBehavior.Cascade);

        // Relationships - Responses (one-to-many)
        builder.HasMany(r => r.Responses)
            .WithOne()
            .HasForeignKey("ReviewId")
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(r => r.SiteId)
            .HasDatabaseName("IX_Reviews_SiteId");

        builder.HasIndex(r => r.UserId)
            .HasDatabaseName("IX_Reviews_UserId");

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

        // Navigation properties metadata
        builder.Metadata
            .FindNavigation(nameof(Review.Images))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata
            .FindNavigation(nameof(Review.Responses))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
