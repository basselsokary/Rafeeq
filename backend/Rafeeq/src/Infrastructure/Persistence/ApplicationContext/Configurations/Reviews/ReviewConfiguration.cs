using Domain.Entities.ReviewAggregate;
using Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.Review;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Reviews;

public sealed class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.Property(r => r.SiteId)
            .IsRequired();

        builder.Property(r => r.TouristId)
            .IsRequired();

        builder.Property(r => r.Title)
            .HasMaxLength(MaxTitleLength)
            .IsRequired();

        builder.Property(r => r.Content)
            .HasMaxLength(MaxContentLength)
            .IsRequired();

        builder.Property(r => r.RejectionReason)
            .IsRequired(false)
            .HasMaxLength(MaxRejectionReasonLength);

        builder.OwnsOne(r => r.Rating, rating =>
        {
            rating.Configure();

            rating.HasIndex(rt => rt.Value)
                .HasDatabaseName("IX_Reviews_Rating");
        });

        builder.HasOne(r => r.Site)
            .WithMany()
            .HasForeignKey(r => r.SiteId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Tourist)
            .WithMany()
            .HasForeignKey(r => r.TouristId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => r.SiteId)
            .HasDatabaseName("IX_Reviews_SiteId");

        builder.HasIndex(r => r.TouristId)
            .HasDatabaseName("IX_Reviews_TouristId");

        builder.HasIndex(r => r.Status)
            .HasDatabaseName("IX_Reviews_Status");

        builder.HasIndex(r => new { r.SiteId, r.Status })
            .HasDatabaseName("IX_Reviews_SiteId_Status");
    }
}
