using Domain.Common;
using Domain.Common.Constants;
using Shared.Models;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public sealed class Email : ValueObject
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9-]+\.[a-zA-Z]{2,}$", RegexOptions.IgnoreCase);

    public string Value { get; } = null!;

    private Email() { }
    private Email(string value)
    {
        Value = value;
    }

    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return EmailErrors.Empty;

        var normalizedEmail = value.Trim().ToLowerInvariant();
        if (normalizedEmail.Length > DomainConstants.User.MaxEmailLength)
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

    public static implicit operator string(Email email) => email.Value;
}
