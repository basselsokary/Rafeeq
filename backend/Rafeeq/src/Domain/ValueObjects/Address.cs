using Domain.Common;

namespace Domain.ValueObjects;

public class Address : ValueObject
{
    public string Street { get; } = null!;
    public string City { get; } = null!;
    public string? Region { get; }
    public string? PostalCode { get; }
    
    private Address() { }
    private Address(string street, string city, string? region, string? postalCode)
    {
        Street = street;
        City = city;
        Region = region;
        PostalCode = postalCode;
    }

    public static Address Create(
        string street,
        string city,
        string? region,
        string? postalCode = null)
    {
        return new Address(street.Trim(), city.Trim(), region?.Trim(), postalCode?.Trim());
    }

    public string GetFullAddress()
    {
        var parts = new List<string> { Street, City };
        
        if (!string.IsNullOrWhiteSpace(Region))
            parts.Add(Region);
        
        if (!string.IsNullOrWhiteSpace(PostalCode))
            parts.Add(PostalCode);

        return string.Join(", ", parts);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return Region ?? string.Empty;
        yield return PostalCode ?? string.Empty;
    }

    public override string ToString()
    {
        return GetFullAddress();
    }
}
