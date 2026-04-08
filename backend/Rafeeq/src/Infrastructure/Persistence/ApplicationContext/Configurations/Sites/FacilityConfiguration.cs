using Domain.Entities.SiteAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Sites;

public sealed class FacilityConfiguration : IEntityTypeConfiguration<Facility>
{
    public void Configure(EntityTypeBuilder<Facility> builder)
    {
        builder.Property(f => f.Name)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(f => f.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(f => f.IsAvailable)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(f => f.CreatedAt)
            .IsRequired();

        builder.Property(f => f.LastModifiedAt);

        builder.HasIndex("SiteId", nameof(Facility.Name))
            .HasDatabaseName("IX_Facilities_SiteId_Name");

        builder.Ignore(f => f.DomainEvents);
    }
}
