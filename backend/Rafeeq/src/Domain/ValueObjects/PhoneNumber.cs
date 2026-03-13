using Domain.Common;
using Shared.Models;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public sealed class PhoneNumber : ValueObject
{
    // private static readonly Regex PhoneRegex = new(@"^\+?[1-9]\d{1,12}$");
    // +201234567890 | 01234567890 | 12345
    private static readonly Regex PhoneRegex = new(@"^((\+201|01)[0-9]\d{8}|\d{5})$");

    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static Result<PhoneNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return PhoneNumberErrors.Empty;

        var normalizedNumber = NormalizeNumber(value.Trim());

        if (!PhoneRegex.IsMatch(normalizedNumber))
            return PhoneNumberErrors.InvalidFormat(value);

        return new PhoneNumber(normalizedNumber);
    }

    private static string NormalizeNumber(string phone)
    {
        if (Regex.IsMatch(phone, @"^\d{5}$"))
            return phone; // short code

        phone = phone.Replace(" ", "").Replace("-", "");

        if (phone.StartsWith("20") && !phone.StartsWith("+"))
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

    public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;
}
