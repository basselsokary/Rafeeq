using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Constants.DomainConstants.Address;

namespace Infrastructure.Data.Application.Configurations.Owened;

public class AddressConfiguration
{
    public static void Configure<T>(OwnedNavigationBuilder<T, Address> ownerBuilder)
        where T : class
    {
        ownerBuilder.Property(a => a.Street)
            .HasColumnName(nameof(Address.Street))
            .IsRequired()
            .HasMaxLength(StreetMaxLength);

        ownerBuilder.Property(a => a.City)
            .HasColumnName(nameof(Address.City))
            .IsRequired()
            .HasMaxLength(CityMaxLength);

        ownerBuilder.Property(a => a.District)
            .HasColumnName(nameof(Address.District))
            .IsRequired()
            .HasMaxLength(DistrictMaxLength);

        ownerBuilder.Property(a => a.ZipCode)
            .HasColumnName(nameof(Address.ZipCode))
            .IsRequired(false)
            .HasMaxLength(ZipCodeMaxLength);
    }
}
