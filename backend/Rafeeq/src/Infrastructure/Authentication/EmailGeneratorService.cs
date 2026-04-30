using System.Globalization;
using System.Text;
using Application.Common.Interfaces.Services;
using Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Authentication;

internal class EmailGeneratorService(
    UserManager<ApplicationUser> context) : IEmailGeneratorService
{
    private const string EmailDomain = "@rafeeq.local";

    public async Task<string> GenerateModeratorEmailAsync(string firstName, string lastName, CancellationToken cancellationToken)
    {
        var normalizedFirst = NormalizeName(firstName);
        var normalizedLast = NormalizeName(lastName);

        var baseEmail = $"{normalizedFirst}.{normalizedLast}{EmailDomain}";

        if (!await EmailExistsAsync(baseEmail, cancellationToken))
        {
            return baseEmail;
        }

        // If the base email already exists, try adding a number suffix
        return await GenerateUniqueEmailWithSuffix(normalizedFirst, normalizedLast, cancellationToken);
    }

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

    private async Task<string> GenerateUniqueEmailWithSuffix(string firstName, string lastName, CancellationToken cancellationToken)
    {
        // Try different strategies
        var strategies = new List<Func<int, string>>
        {
            // Strategy 1: firstname.lastname2@rafeeq.com
            counter => $"{firstName}.{lastName}{counter}{EmailDomain}",
            
            // Strategy 2: firstname.l@rafeeq.com (use last name initial)
            counter => $"{firstName}.{lastName[0]}{counter}{EmailDomain}",
            
            // Strategy 3: f.lastname@rafeeq.com (use first name initial)
            counter => $"{firstName[0]}.{lastName}{counter}{EmailDomain}",
            
            // Strategy 4: Add random short string
            counter => $"{firstName}.{lastName}.{GenerateShortId()}{EmailDomain}"
        };
        
        foreach (var strategy in strategies)
        {
            for (int counter = 2; counter <= 99; counter++)
            {
                var email = strategy(counter);
                if (!await EmailExistsAsync(email, cancellationToken))
                {
                    return email;
                }
            }
        }

        // Fallback: use timestamp
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        return $"{firstName}.{lastName}.{timestamp}{EmailDomain}";
    }

    private async Task<string> GenerateUniqueUsernameWithSuffix(string firstName, string lastName, CancellationToken cancellationToken)
    {
        // Try adding numbers
        for (int counter = 101; counter <= 9999; counter++)
        {
            var username = $"{firstName}{lastName}{counter}";
            if (!await UsernameExistsAsync(username, cancellationToken))
            {
                return username;
            }
        }

        // Fallback
        var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 6);
        return $"{firstName}{lastName}{uniqueId}";
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

    private string GenerateShortId()
    {
        // Generate a short random alphanumeric string (6 chars)
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken)
    {
        string normalizedUsername = context.NormalizeName(username);
        return await context.Users.AnyAsync(u => u.NormalizedUserName == normalizedUsername, cancellationToken);
    }

    private async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
    {
        string normalizedEmail = context.NormalizeEmail(email);
        return await context.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
    }
}
