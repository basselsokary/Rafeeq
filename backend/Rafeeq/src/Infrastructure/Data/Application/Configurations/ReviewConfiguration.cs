using Domain.Entities.PlaceAggregate;
using Domain.Entities.ReviewAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.Review;

namespace Infrastructure.Data.Application.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.Property(r => r.Rating)
            .IsRequired();

        builder.Property(r => r.Text)
            .IsRequired(false)
            .HasMaxLength(MaxTextLength);

        builder.HasOne<Place>()
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.AttractionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}