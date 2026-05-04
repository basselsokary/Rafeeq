using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;

internal static class PhoneNumberConfiguration
{
    public static void Configure<T>(
        this OwnedNavigationBuilder<T, PhoneNumber> builder) where T : class
    {
        builder.Property(p => p.Value)
            .HasColumnName("ContactPhone")
            .HasMaxLength(PhoneNumber.MaxPhoneNumberLength);
    }
}