using Domain.Common;
using Domain.Common.Constants;
using Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

/// <summary>
/// Represents an email address
/// </summary>
public sealed class Email : ValueObject
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9-]+\.[a-zA-Z]{2,}$", RegexOptions.IgnoreCase);

    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new BusinessRuleValidationException("Email cannot be empty.");

        var normalizedEmail = value.Trim().ToLowerInvariant();
        if (normalizedEmail.Length > DomainConstants.User.MaxEmailLength) 
            throw new BusinessRuleValidationException("Email is too long.");

        if (!EmailRegex.IsMatch(normalizedEmail))
            throw new BusinessRuleValidationException($"'{value}' is not a valid email address.");

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
