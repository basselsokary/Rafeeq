using Domain.Entities.SiteAggregate;
using Domain.Entities.TripAggregate;
using Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.Trip;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Trips;

internal sealed class TripSiteConfiguration : IEntityTypeConfiguration<TripSite>
{
    public void Configure(EntityTypeBuilder<TripSite> builder)
    {
        builder.ToTable("TripSites");

        builder.Property(ts => ts.VisitDate)
            .IsRequired();

        builder.OwnsOne(ts => ts.VisitTimeRange, timeRange =>
        {
            timeRange.Configure();
        });

        builder.Property(ts => ts.EstimatedDurationMinutes)
            .IsRequired();

        builder.Property(ts => ts.DisplayOrder)
            .IsRequired();

        builder.Property(ts => ts.IsVisited)
            .IsRequired();

        builder.Property(ts => ts.ActualVisitTime)
            .IsRequired(false);

        builder.Property(ts => ts.ActualDurationMinutes)
            .IsRequired(false);

        builder.Property(ts => ts.Notes)
            .HasMaxLength(MaxNoteLength)
            .IsRequired(false);

        builder.HasOne<Site>()
            .WithMany()
            .HasForeignKey(ts => ts.SiteId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex("TripId", nameof(TripSite.DisplayOrder))
            .HasDatabaseName("IX_TripSites_TripId_DisplayOrder");

        builder.HasIndex(ts => ts.SiteId)
            .HasDatabaseName("IX_TripSites_SiteId");

        builder.HasIndex("TripId", nameof(TripSite.VisitDate))
            .HasDatabaseName("IX_TripSites_TripId_VisitDate");
    }
}
