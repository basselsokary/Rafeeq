using Domain.Entities.SiteAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Sites;

public sealed class NearestTransportationConfiguration : IEntityTypeConfiguration<NearestTransportation>
{
    public void Configure(EntityTypeBuilder<NearestTransportation> builder)
    {
        builder.Property(t => t.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Type)
            .HasConversion<string>()
            .HasMaxLength(24)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.LastModifiedAt);

        builder.OwnsOne(t => t.Location, location =>
        {
            location.Property(l => l.Latitude)
                .HasColumnName("Latitude")
                .HasPrecision(9, 6)
                .IsRequired();

            location.Property(l => l.Longitude)
                .HasColumnName("Longitude")
                .HasPrecision(9, 6)
                .IsRequired();
        });

        builder.OwnsOne(t => t.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("Street")
                .HasMaxLength(200);

            address.Property(a => a.City)
                .HasColumnName("City")
                .HasMaxLength(100);

            address.Property(a => a.Region)
                .HasColumnName("Region")
                .HasMaxLength(100);

            address.Property(a => a.PostalCode)
                .HasColumnName("PostalCode")
                .HasMaxLength(20);
        });

        builder.Navigation(t => t.Address).IsRequired(false);

        builder.OwnsOne(t => t.OperatingHours, hours =>
        {
            hours.Property(h => h.StartTime)
                .HasColumnName("OperatingStartTime");

            hours.Property(h => h.EndTime)
                .HasColumnName("OperatingEndTime");
        });

        builder.Navigation(t => t.OperatingHours).IsRequired(false);

        builder.HasIndex("SiteId", nameof(NearestTransportation.Type))
            .HasDatabaseName("IX_NearestTransportations_SiteId_Type");

        builder.Ignore(t => t.DomainEvents);
    }
}
