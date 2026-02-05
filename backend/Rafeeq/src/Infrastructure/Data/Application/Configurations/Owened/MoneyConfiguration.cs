using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Constants.DomainConstants.Money;

namespace Infrastructure.Data.Configurations.Owened;

public class MoneyConfiguration
{
    public static void Configure<T>(OwnedNavigationBuilder<T, Money> ownerBuilder)
        where T : class
    {
        ownerBuilder.Property(m => m.Amount)
            .HasColumnName(nameof(Money.Amount))
            .HasPrecision(Precision, Scale)
            .IsRequired();

        ownerBuilder.Property(m => m.Currency)
            .HasColumnName(nameof(Money.Currency))
            .HasMaxLength(MaxCurrencyLength)
            .IsRequired();
    }
}