using Domain.Entities;
using Infrastructure.Data.Configurations.Owened;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Application.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.OwnsOne(
            oi => oi.Amount,
            b => MoneyConfiguration.Configure(b));

        builder.Property(p => p.PaymentDate)
            .IsRequired(false);

        builder.Property(p => p.Method)
            .IsRequired();

        builder.Property(p => p.Status)
            .IsRequired();

        // builder.HasOne<Customer>()
        //     .WithMany()
        //     .HasForeignKey(p => p.CustomerId)
        //     .IsRequired()
        //     .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Order)
            .WithOne()
            .HasForeignKey<Payment>(p => p.OrderId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}