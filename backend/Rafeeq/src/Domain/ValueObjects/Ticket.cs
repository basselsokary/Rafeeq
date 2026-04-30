using Domain.Common;
using Shared;

namespace Domain.ValueObjects;

public sealed class Ticket : ValueObject
{
    public Money EgyptianPrice { get; } = null!;
    public Money? ForeignerPrice { get; }

    private Ticket() { }
    private Ticket(Money egyptianPrice, Money? foreignerPrice)
    {
        EgyptianPrice = egyptianPrice;
        ForeignerPrice = foreignerPrice;
    }

    public static Result<Ticket> Create(Money egyptianPrice, Money? foreignerPrice)
    {
        return new Ticket(egyptianPrice, foreignerPrice);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return EgyptianPrice;
        yield return ForeignerPrice ?? Money.Create(0, "USD").Value;
    }

    public override string ToString()
    {
        if (EgyptianPrice?.Amount == 0 && ForeignerPrice?.Amount == 0)
            return "Free";

        if (EgyptianPrice?.Amount > 0 && ForeignerPrice?.Amount > 0)
            return $"Egyptian: EGP {EgyptianPrice.Amount:N2}\nForeigner: {ForeignerPrice.Currency} {ForeignerPrice.Amount:N2}";

        if (EgyptianPrice?.Amount > 0)
            return $"Egyptian: EGP {EgyptianPrice.Amount:N2}";

        return $"Foreigner: {ForeignerPrice?.Currency} {ForeignerPrice?.Amount:N2}";
    }
}
