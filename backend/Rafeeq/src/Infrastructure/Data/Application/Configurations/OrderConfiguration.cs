using Domain.Entities;
using Infrastructure.Data.Application.Configurations.Owened;
using Infrastructure.Data.Configurations.Owened;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Application.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(o => o.Status)
            .IsRequired();

        builder.Property(o => o.DeliveredAt)
            .IsRequired(false);

        builder.Property(o => o.ShippedAt)
            .IsRequired(false);

        builder.OwnsOne(
            o => o.Address,
            b => AddressConfiguration.Configure(b));

        builder.OwnsOne(
            o => o.TotalAmount,
            b => MoneyConfiguration.Configure(b));

        // builder.HasOne<Customer>()
        //     .WithMany()
        //     .HasForeignKey(o => o.CustomerId)
        //     .IsRequired()
        //     .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.OrderItems)
            .WithOne()
            .HasForeignKey(oi => oi.OrderId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
