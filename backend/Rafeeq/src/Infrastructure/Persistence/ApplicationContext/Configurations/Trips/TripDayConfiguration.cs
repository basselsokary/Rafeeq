using Domain.Entities.TripAggregate;
using Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.Trip;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Trips;

internal sealed class TripDayConfiguration : IEntityTypeConfiguration<TripDay>
{
    public void Configure(EntityTypeBuilder<TripDay> builder)
    {
        builder.Property(td => td.DayNumber)
            .IsRequired();

        builder.Property(ts => ts.Notes)
            .HasMaxLength(MaxNoteLength)
            .IsRequired(false);

        builder.Property(td => td.Date)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(td => td.EstimatedDayDuration)
            .HasConversion(
                v => (int)v.TotalMinutes,
                v => TimeSpan.FromMinutes(v))
            .HasColumnName("DayTotalDurationMinutes")
            .IsRequired();
        
        builder.OwnsOne(s => s.EstimatedDayBudget, budget =>
        {
            budget.Configure();
        });

        builder.HasMany(td => td.Sites)
            .WithOne()
            .HasForeignKey("TripDayId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex("TripId", nameof(TripDay.Date))
            .IsUnique()
            .HasDatabaseName("IX_TripDays_TripId_Date");
        
        builder.HasIndex(td => td.Date)
            .HasDatabaseName("IX_TripDays_Date");
    }
}
