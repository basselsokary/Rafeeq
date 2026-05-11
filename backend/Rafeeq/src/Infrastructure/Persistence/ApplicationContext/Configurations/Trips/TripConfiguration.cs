using System.Text.Json;
using Domain.Entities.TripAggregate;
using Domain.Enums;
using Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.Trip;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Trips;

internal sealed class TripConfiguration : IEntityTypeConfiguration<Trip>
{
    public void Configure(EntityTypeBuilder<Trip> builder)
    {
        builder.ToTable("Trips");

        builder.Property(t => t.TouristId)
            .IsRequired();

        builder.Property(t => t.Title)
            .HasMaxLength(MaxNameLength)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(MaxDescriptionLength)
            .IsRequired(false);

        builder.Property(t => t.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(t => t.EstimatedTotalDuration)
            .HasConversion(
                v => (int)v.TotalMinutes,
                v => TimeSpan.FromMinutes(v))
            .HasColumnName("EstimatedTotalDurationMinutes")
            .IsRequired();

        builder.Property(t => t.StartDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(t => t.EndDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(t => t.DailyStartTime)
            .HasColumnType("time")
            .IsRequired();

        builder.Property(t => t.DailyEndTime)
            .HasColumnType("time")
            .IsRequired();

        builder.OwnsOne(s => s.UserPosition, location =>
        {
            location.Configure("UserPosition");

            location.HasIndex(l => new { l.Latitude, l.Longitude })
                .HasDatabaseName("IX_Trips_UserPosition_LatLng");
        });

        builder.OwnsOne(t => t.EstimatedTotalBudget, estimatedBudget =>
        {
            estimatedBudget.Configure("EstimatedBudget");
        });

        builder.OwnsOne(t => t.ActualCost, actualCost =>
        {
            actualCost.Configure("ActualCost");
        });

        builder.Property<List<SiteType>>("_preferredSiteTypes")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<List<SiteType>>(v, (JsonSerializerOptions)null!)!)
            .HasColumnName("PreferredSiteTypes")
            .HasColumnType("nvarchar(1024)")
            .Metadata.SetValueComparer(new ValueComparer<List<SiteType>>(
                (c1, c2) => c1!.OrderBy(x => x).SequenceEqual(c2!.OrderBy(x => x)),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
            ));
        
        builder.HasMany(t => t.Days)
            .WithOne()
            .HasForeignKey("TripId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.Title)
            .HasDatabaseName("IX_Trips_Name");
        
        builder.HasIndex(t => t.TouristId)
            .HasDatabaseName("IX_Trips_TouristId");

        builder.HasIndex(t => t.Status)
            .HasDatabaseName("IX_Trips_Status");
        
        builder.HasIndex(t => t.StartDate)
            .HasDatabaseName("IX_Trips_StartDate");
    }
}
