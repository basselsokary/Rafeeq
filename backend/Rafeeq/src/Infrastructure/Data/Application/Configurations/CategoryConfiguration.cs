using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.Category;

namespace Infrastructure.Data.Application.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(MaxNameLength);

        builder.Property(c => c.Description)
            .IsRequired(false)
            .HasMaxLength(MaxDescriptionLength);
    }
}