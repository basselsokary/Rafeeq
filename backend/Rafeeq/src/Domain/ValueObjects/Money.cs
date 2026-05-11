using Domain.Common;
using Shared;

namespace Domain.ValueObjects;

public sealed class Money : ValueObject 
{
    public decimal Amount { get; }
    public string Currency { get; } = null!;
    
    public static Money Zero => new(0, "EGP");

    private Money() { }
    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Result<Money> Create(decimal amount, string currency = "EGP")
    {
        if (amount < 0)
            return MoneyErrors.NegativeAmount;

        if (string.IsNullOrWhiteSpace(currency))
            return MoneyErrors.CurrencyRequired;

        if (currency.Length != 3)
            return MoneyErrors.InvalidCurrencyFormat;

        return new Money(amount, currency.ToUpperInvariant());
    }

    public Result<Money> Add(Money other)
    {
        if (Currency != other.Currency)
            return MoneyErrors.CurrencyMismatch;

        return new Money(Amount + other.Amount, Currency);
    }

    /// <summary>
    /// This method does not handle currency conversion. It assumes both Money instances are in the same currency.
    /// </summary>
    public Money Add(decimal amount)
    {
        return new Money(Amount + amount, Currency);
    }

    public Result<Money> Subtract(Money other)
    {
        if (Currency != other.Currency)
            return MoneyErrors.CurrencyMismatch;

        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal factor)
    {
        return new Money(Amount * factor, Currency);
    }

    public Result<Money> Divide(decimal divisor)
    {
        if (divisor == 0)
            return MoneyErrors.DivisionByZero;

        return new Money(Amount / divisor, Currency);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Currency} {Amount:N2}";

    public static bool operator >(Money a, Money b) => a.Amount > b.Amount;
    public static bool operator <(Money a, Money b) => b > a;

    public static bool operator >=(Money a, Money b) => a.Amount >= b.Amount;
    public static bool operator <=(Money a, Money b) => b <= a;
}
