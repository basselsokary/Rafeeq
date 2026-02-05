using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Application.Configurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.HasMany(c => c.CartItems)
            .WithOne()
            .HasForeignKey(ci => ci.CartId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // builder.HasOne<Customer>()
        //     .WithOne()
        //     .HasForeignKey<Cart>(c => c.CustomerId)
        //     .IsRequired()
        //     .OnDelete(DeleteBehavior.Cascade);
    }
}