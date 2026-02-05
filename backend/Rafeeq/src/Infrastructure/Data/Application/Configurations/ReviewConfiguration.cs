using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Constants.DomainConstants.Review;

namespace Infrastructure.Data.Application.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.Property(r => r.Rating)
            .IsRequired();

        builder.Property(r => r.Comment)
            .IsRequired(false)
            .HasMaxLength(CommentMaxLength);

        // builder.HasOne<Customer>()
        //     .WithMany()
        //     .HasForeignKey(r => r.CustomerId)
        //     .IsRequired()
        //     .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Product>()
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.ProductId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}