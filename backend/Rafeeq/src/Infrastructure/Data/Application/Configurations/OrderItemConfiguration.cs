using Domain.Entities;
using Infrastructure.Data.Configurations.Owened;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Application.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.Property(oi => oi.Quantity)
            .IsRequired();

        builder.OwnsOne(
            oi => oi.UnitPrice,
            b => MoneyConfiguration.Configure(b));

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(oi => oi.ProductId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}