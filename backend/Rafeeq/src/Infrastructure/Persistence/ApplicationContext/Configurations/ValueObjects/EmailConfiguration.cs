using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;

internal static class EmailConfiguration
{
    public static void Configure<T>(
        this OwnedNavigationBuilder<T, Email> builder) where T : class
    {
        builder.Property(e => e.Value)
            .HasColumnName("ContactEmail")
            .HasMaxLength(Email.MaxEmailLength);
    }
}
