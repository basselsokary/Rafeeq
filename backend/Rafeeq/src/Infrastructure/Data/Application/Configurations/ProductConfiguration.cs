using Domain.Entities;
using Infrastructure.Data.Configurations.Owened;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Constants.DomainConstants.Product;

namespace Infrastructure.Data.Application.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(NameMaxLength);

        builder.Property(p => p.Description)
            .IsRequired(false)
            .HasMaxLength(DescriptionMaxLength);

        builder.Property(p => p.UnitsInStock)
            .IsRequired();

        builder.Property(p => p.ImageURL)
            .IsRequired(false)
            .HasMaxLength(ImageUrlMaxLength);

        builder.OwnsOne(
            p => p.Price,
            b => MoneyConfiguration.Configure(b));
    }
}
