using Domain.Common;
using Shared;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public sealed class Email : ValueObject
{
    public const int MaxEmailLength = 254; // Maximum length of emails is 254

    // For examples: example@domain.com, user.name@company.org, test.email+tag@domain.co.uk, 
    private static readonly Regex EmailRegex = new(
        @"^(?!\.)(?!.*\.\.)([a-zA-Z0-9._%+-]{1,64})(?<!\.)@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.IgnoreCase);

    public string Value { get; } = null!;

    private Email() { }
    private Email(string value)
    {
        Value = value;
    }

    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return EmailErrors.Required;

        var normalizedEmail = value.Trim().ToLowerInvariant();
        if (normalizedEmail.Length > MaxEmailLength)
            return EmailErrors.TooLong;

        if (!EmailRegex.IsMatch(normalizedEmail))
            return EmailErrors.InvalidFormat(value);

        return new Email(normalizedEmail);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator string(Email? email) => email?.Value ?? string.Empty;
}
