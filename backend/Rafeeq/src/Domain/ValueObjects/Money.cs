using Domain.Common;
using Domain.Common.Exceptions;

namespace Domain.ValueObjects;

public class Money : ValueObject 
{
    public decimal Amount { get; }
    public string Currency { get; } = string.Empty;
    
    private Money() { }
    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, string currency = "EGP")
    {
        if (amount < 0)
            throw new BusinessRuleValidationException("Amount cannot be negative.");

        if (string.IsNullOrWhiteSpace(currency))
            throw new BusinessRuleValidationException("Currency is required.");

        if (currency.Length != 3)
            throw new BusinessRuleValidationException("Currency must be a 3-letter ISO code.");

        return new(amount, currency);
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationDomainException("Cannot add different currencies.");

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationDomainException("Cannot subtract different currencies.");

        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal factor)
    {
        return new Money(Amount * factor, Currency);
    }

    public Money Divide(decimal divisor)
    {
        if (divisor == 0)
            throw new InvalidOperationDomainException("Cannot divide by 0.");

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