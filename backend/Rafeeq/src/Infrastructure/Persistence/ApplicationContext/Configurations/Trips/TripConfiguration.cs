using Domain.Entities.TripAggregate;
using Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;
using Microsoft.EntityFrameworkCore;
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

        builder.Property(t => t.Name)
            .HasMaxLength(MaxNameLength)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(MaxDescriptionLength)
            .IsRequired(false);

        builder.Property(t => t.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(t => t.PreferredTransportation)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(t => t.EstimatedTotalDurationMinutes)
            .IsRequired();

        builder.Property(t => t.ShareCount)
            .IsRequired();

        builder.Property(t => t.IsPublic)
            .IsRequired();

        builder.OwnsOne(t => t.DateRange, dateRange =>
        {
            dateRange.Configure();
        });

        builder.OwnsOne(t => t.EstimatedBudget, estimatedBudget =>
        {
            estimatedBudget.Configure("EstimatedBudget");
        });

        builder.OwnsOne(t => t.ActualCost, actualCost =>
        {
            actualCost.Configure("ActualCost");
        });
        
        builder.HasMany(t => t.Sites)
            .WithOne()
            .HasForeignKey("TripId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(t => t.Notes)
            .WithOne()
            .HasForeignKey("TripId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.Name)
            .HasDatabaseName("IX_Trips_Name");
        
        builder.HasIndex(t => t.TouristId)
            .HasDatabaseName("IX_Trips_TouristId");

        builder.HasIndex(t => t.Status)
            .HasDatabaseName("IX_Trips_Status");

        builder.HasIndex(t => t.IsPublic)
            .HasDatabaseName("IX_Trips_IsPublic");
    }
}
