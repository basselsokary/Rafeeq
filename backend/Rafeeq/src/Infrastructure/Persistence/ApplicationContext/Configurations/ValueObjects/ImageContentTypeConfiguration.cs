using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;

internal static class ImageContentTypeConfiguration
{
    public static void Configure<T>(
        this OwnedNavigationBuilder<T, ImageContentType> builder) where T : class
    {
        builder.Property(k => k.Value)
            .HasColumnName("ContentType")
            .HasMaxLength(Domain.Common.Constants.DomainConstants.File.MaxContentTypeLength);
    }
}
