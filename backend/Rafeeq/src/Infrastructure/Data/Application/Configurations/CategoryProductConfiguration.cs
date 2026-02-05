using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Application.Configurations;

public class CategoryProductConfiguration : IEntityTypeConfiguration<CategoryProduct>
{
    public void Configure(EntityTypeBuilder<CategoryProduct> builder)
    {
        builder.HasKey(cp => new { cp.CategoryId, cp.ProductId });

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(cp => cp.CategoryId)
            .IsRequired();

        builder.HasOne<Product>()
            .WithMany(p => p.CategoryProducts)
            .HasForeignKey(cp => cp.ProductId)
            .IsRequired();
    }
}