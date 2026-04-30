using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;

internal static class TimeRangeConfiguration
{
    public static void Configure<T>(
        this OwnedNavigationBuilder<T, TimeRange> builder) where T : class
    {
        builder.Property(h => h.StartTime)
            .HasColumnName("StartTime");

        builder.Property(h => h.EndTime)
            .HasColumnName("EndTime");
    }
}