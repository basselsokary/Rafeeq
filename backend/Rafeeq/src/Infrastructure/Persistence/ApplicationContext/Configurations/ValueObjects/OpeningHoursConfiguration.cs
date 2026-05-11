using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;

internal static class OpeningHoursConfiguration
{
    public static void Configure<T>(
        this OwnedNavigationBuilder<T, OpeningHour> builder,
        string prefixEntityName) where T : class
    {
        builder.ToTable($"{prefixEntityName}_OpeningHours");
        builder.WithOwner().HasForeignKey($"{prefixEntityName}Id");

        builder.Property<int>("Id");
        builder.HasKey("Id");

        builder.OwnsOne(oh => oh.OpeningTime, timeRange =>
        {
            timeRange.Configure();
        });

        builder.Property(oh => oh.IsClosed)
            .HasColumnName("IsClosed")
            .IsRequired();
    }
}
