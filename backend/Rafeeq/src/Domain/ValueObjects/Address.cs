using Domain.Common;
using Shared;

namespace Domain.ValueObjects;

public class Address : ValueObject
{
    public string Value { get; } = null!;
    
    private Address() { }
    private Address(string value)
    {
        Value = value;
    }

    public static Result<Address> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return AddressErrors.EmptyAddress;

        return new Address(value.Trim());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator string(Address? address) => address?.Value ?? string.Empty;
}
