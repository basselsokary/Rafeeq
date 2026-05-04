using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.Money;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;

internal static class MoneyConfiguration
{
    public static void Configure<T>(
        this OwnedNavigationBuilder<T, Money> ownerBuilder,
        string prefix = "Price") where T : class
    {
        ownerBuilder.Property(m => m.Amount)
            .HasColumnName($"{prefix}_Amount")
            .HasPrecision(Precision, Scale);

        ownerBuilder.Property(m => m.Currency)
            .HasColumnName($"{prefix}_Currency")
            .HasMaxLength(MaxCurrencyLength);
    }
}