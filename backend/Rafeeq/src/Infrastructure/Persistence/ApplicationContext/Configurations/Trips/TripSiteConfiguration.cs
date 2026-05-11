using Domain.Entities.SiteAggregate;
using Domain.Entities.TripAggregate;
using Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.Trip;
using static Domain.Common.Constants.DomainConstants.File;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Trips;

internal sealed class TripSiteConfiguration : IEntityTypeConfiguration<TripSite>
{
    public void Configure(EntityTypeBuilder<TripSite> builder)
    {
        builder.ToTable("TripSites");

        builder.Property(c => c.SiteName)
            .HasMaxLength(MaxNameLength)
            .IsRequired();

        builder.Property(c => c.SiteImageUrl)
            .HasMaxLength(MaxImageUrlLength)
            .IsRequired();

        builder.Property(c => c.CityName)
            .HasMaxLength(MaxNameLength)
            .IsRequired();

        builder.Property(c => c.SiteType)
            .HasConversion<int>()
            .HasColumnName("Type")
            .IsRequired();

        builder.Property(ts => ts.PlannedArrivalTime)
            .HasColumnType("time")
            .IsRequired();

        builder.OwnsOne(s => s.SiteLocation, location =>
        {
            location.Configure("SiteLocation");

            location.HasIndex(l => new { l.Latitude, l.Longitude })
                .HasDatabaseName("IX_TripSites_SiteLocation_LatLng");
        });

        builder.OwnsOne(s => s.EstimatedCost, cost =>
        {
            cost.Configure();
        });

        builder.Property(ts => ts.EstimatedDuration)
            .HasConversion(
                v => (int)v.TotalMinutes,
                v => TimeSpan.FromMinutes(v))
            .HasColumnName("EstimatedDurationMinutes")
            .IsRequired();

        builder.Property(ts => ts.VisitOrder)
            .IsRequired();

        builder.Property(ts => ts.IsVisited)
            .IsRequired();

        builder.Property(ts => ts.ActualVisitTime)
            .IsRequired(false);

        builder.HasOne<Site>()
            .WithMany()
            .HasForeignKey(ts => ts.SiteId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex("TripDayId", nameof(TripSite.VisitOrder))
            .HasDatabaseName("IX_TripSites_TripId_DisplayOrder");

        builder.HasIndex(ts => ts.SiteId)
            .HasDatabaseName("IX_TripSites_SiteId");

        builder.HasIndex("TripDayId", nameof(TripSite.PlannedArrivalTime))
            .HasDatabaseName("IX_TripSites_TripId_PlannedArrivalTime");
    }
}
