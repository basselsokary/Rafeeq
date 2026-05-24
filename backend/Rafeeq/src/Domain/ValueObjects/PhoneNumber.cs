using Domain.Common;
using Shared;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public sealed class PhoneNumber : ValueObject
{
    public const int MaxPhoneNumberLength = 20;
    
    // +201234567890 | 201234567890 | 01234567890 | 12345 | +2012345 | 2012345
    // private static readonly Regex PhoneRegex = new(@"^((\+201|201|01)[0-9]\d{8}|\d{5}|(\+20|20)[0-9]\d{4})$");

    public string Value { get; } = null!;

    private PhoneNumber() { }
    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static Result<PhoneNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return PhoneNumberErrors.Empty;

        var normalizedNumber = NormalizeNumber(value.Trim());

        // if (!PhoneRegex.IsMatch(normalizedNumber))
        //     return PhoneNumberErrors.InvalidFormat(value);

        return new PhoneNumber(normalizedNumber);
    }

    private static string NormalizeNumber(string phone)
    {
        if (Regex.IsMatch(phone, @"^\d{5}$"))
            return phone; // short code

        phone = phone.Replace(" ", "").Replace("-", "");

        if (phone.StartsWith("20") && !phone.StartsWith('+'))
            phone = "+" + phone;

        if (phone.StartsWith("01"))
            phone = "+2" + phone;

        return phone;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator string(PhoneNumber? phoneNumber) => phoneNumber?.Value ?? string.Empty;
}
