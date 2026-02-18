using Domain.Common;
using Domain.Common.Exceptions;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public sealed class PhoneNumber : ValueObject
{
    private static readonly Regex PhoneRegex = new(@"^\+?[1-9]\d{1,12}$");

    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static PhoneNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new BusinessRuleValidationException("Phone number cannot be empty.");

        var cleanedNumber = Regex.Replace(value, @"[^\d+]", "");

        if (!PhoneRegex.IsMatch(cleanedNumber))
            throw new BusinessRuleValidationException($"'{value}' is not a valid phone number.");

        return new PhoneNumber(cleanedNumber);
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
