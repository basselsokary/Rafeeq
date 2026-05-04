using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;

internal static class RatingConfiguration
{
    public static void Configure<T>(
        this OwnedNavigationBuilder<T, Rating> builder) where T : class
    {
        builder.Property(x => x.Value)
            .HasColumnName("Rating");
    }
}
