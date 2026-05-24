using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Application.Common.Interfaces.Services;
using Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Authentication;

internal class UserCredentialService(
    UserManager<ApplicationUser> context) : IUserCredentialService
{
    public async Task<string> GenerateUniqueUsernameAsync(string firstName, string lastName, CancellationToken cancellationToken)
    {
        var normalizedFirst = NormalizeName(firstName);
        var normalizedLast = NormalizeName(lastName);

        var baseUsername = $"{normalizedFirst}{normalizedLast}";

        if (!await UsernameExistsAsync(baseUsername, cancellationToken))
        {
            return baseUsername;
        }

        // If the base username already exists, try adding a number suffix
        return await GenerateUniqueUsernameWithSuffix(normalizedFirst, normalizedLast, cancellationToken);
    }

    public string GenerateTemporaryPassword()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
        const string numbers = "23456789";
        const string specialChars = "!@#$%^&*";

        var random = RandomNumberGenerator.Create();
        var data = new byte[16];
        random.GetBytes(data);

        var password = new StringBuilder();
        password.Append("Raf"); // Prefix to indicate temporary

        for (int i = 0; i < 6; i++)
        {
            password.Append(chars[data[i] % chars.Length]);
        }

        for (int i = 0; i < 1; i++)
        {
            password.Append(specialChars[data[i] % specialChars.Length]);
        }

        for (int i = 0; i < 3; i++)
        {
            password.Append(numbers[data[i] % numbers.Length]);
        }

        return password.ToString();
    }

    private string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty");

        // Remove Arabic diacritics, spaces, special characters
        var normalized = name
            .Trim()
            .ToLowerInvariant()
            .Replace(" ", "")
            .Replace("-", "")
            .Replace("'", "")
            .Replace(".", "");

        // Remove accents (for names like José → jose)
        normalized = RemoveDiacritics(normalized);

        // Remove any remaining non-alphanumeric characters
        normalized = new string(normalized.Where(char.IsLetterOrDigit).ToArray());

        return normalized;
    }

    private async Task<string> GenerateUniqueUsernameWithSuffix(
        string firstName,
        string lastName,
        CancellationToken cancellationToken)
    {
        var baseUsername = $"{firstName}{lastName}";

        // Fetch all taken suffixes for this base in one query
        var takenSuffixes = await context.Users
            .AsNoTracking()
            .Where(u => u.NormalizedUserName!.StartsWith(baseUsername.ToUpperInvariant()))
            .Select(u => u.NormalizedUserName!)
            .ToHashSetAsync(cancellationToken);

        // Check numeric suffixes in-memory
        for (int counter = 101; counter <= 9999; counter++)
        {
            var candidate = $"{baseUsername}{counter}";
            if (!takenSuffixes.Contains(candidate.ToUpperInvariant()))
                return candidate;
        }

        baseUsername = $"{lastName}{firstName}";
        string username;
        do
        {
            username = $"{baseUsername}{Guid.NewGuid():N}"[..^26]; // keeps 6 hex chars
        } while (takenSuffixes.Contains(username.ToUpperInvariant()));

        return username;
    }

    private string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    private async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken)
    {
        string normalizedUsername = context.NormalizeName(username);
        return await context.Users.AnyAsync(u => u.NormalizedUserName == normalizedUsername, cancellationToken);
    }
}
