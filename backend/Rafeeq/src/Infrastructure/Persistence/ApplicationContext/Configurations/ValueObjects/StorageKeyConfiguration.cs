using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;

internal static class StorageKeyConfiguration
{
    public static void Configure<T>(
        this OwnedNavigationBuilder<T, StorageKey> builder) where T : class
    {
        builder.Property(k => k.Value)
            .HasColumnName("StorageKey")
            .HasMaxLength(Domain.Common.Constants.DomainConstants.Image.MaxStorageKeyLength)
            .IsRequired();
    }
}
