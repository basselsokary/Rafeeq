using Domain.Entities.SiteAggregate;
using Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Sites;

internal sealed class NearestTransportationConfiguration : IEntityTypeConfiguration<NearestTransportation>
{
    public void Configure(EntityTypeBuilder<NearestTransportation> builder)
    {
        builder.OwnsOne(t => t.Location, location =>
        {
            location.Configure();
        });

        builder.OwnsOne(t => t.OperatingHours, hours =>
        {
            hours.Configure();
        });

        builder.HasMany(s => s.LocalizedContents)
            .WithOne()
            .HasForeignKey("TransportationId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex("SiteId", nameof(NearestTransportation.Type))
            .HasDatabaseName("IX_NearestTransportations_SiteId_Type");
    }
}
