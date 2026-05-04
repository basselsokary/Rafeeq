using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.GeoLocation;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;

internal static class GeoLocationConfiguration
{
    public static void Configure<T>(
        this OwnedNavigationBuilder<T, GeoLocation> builder,
        string prefix = "Location") where T : class
    {
        builder.Property(x => x.Latitude)
            .HasColumnName($"{prefix}_Latitude")
            .HasPrecision(Precision, Scale);

        builder.Property(x => x.Longitude)
            .HasColumnName($"{prefix}_Longitude")
            .HasPrecision(Precision, Scale);
    }
}
