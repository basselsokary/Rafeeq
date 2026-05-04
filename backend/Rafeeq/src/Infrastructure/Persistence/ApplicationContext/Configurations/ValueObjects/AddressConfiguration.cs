using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.Address;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;

internal static class AddressConfiguration
{
    public static void Configure<T>(
        this OwnedNavigationBuilder<T, Address> builder) where T : class
    {
        builder.Property(a => a.Value)
            .HasColumnName("Address")
            .HasMaxLength(MaxStreetLength);
    }
}
