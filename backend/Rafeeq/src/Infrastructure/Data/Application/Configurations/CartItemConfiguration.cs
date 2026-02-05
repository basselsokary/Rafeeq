using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Application.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.Property(ci => ci.Quantity)
            .IsRequired();

        builder.HasOne<Product>()
            .WithOne()
            .HasForeignKey<CartItem>(ci => ci.ProductId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
