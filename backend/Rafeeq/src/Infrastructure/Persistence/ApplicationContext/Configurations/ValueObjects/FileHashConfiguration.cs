using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;

internal static class FileHashConfiguration
{
    public static void Configure<T>(
        this OwnedNavigationBuilder<T, FileHash> builder) where T : class
    {
        builder.Property(f => f.Value)
            .HasColumnName("Hash")
            .HasMaxLength(Domain.Common.Constants.DomainConstants.File.MaxHashLength);
    }
}
