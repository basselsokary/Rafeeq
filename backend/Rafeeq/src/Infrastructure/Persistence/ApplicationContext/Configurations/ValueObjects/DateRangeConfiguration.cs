using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;

internal static class DateRangeConfiguration
{
    public static void Configure<T>(
        this OwnedNavigationBuilder<T, DateRange> builder) where T : class
    {
        builder.Property(h => h.StartDate)
            .HasColumnName("StartDate");

        builder.Property(h => h.EndDate)
            .HasColumnName("EndDate");
    }
}
