using Domain.ValueObjects;
using Shared;

namespace Infrastructure.Persistence.Seeding.Parsers;

/// <summary>
/// Parses the <c>Discount Amount 'NULLABLE'</c> CSV cell into a <see cref="Money"/> value object.
///
/// Format: plain decimal or decimal with a leading/trailing currency symbol.
/// Examples:
///   "50"    → EGP 50
///   "8$"    → USD 8
///   "€15"   → EUR 15
///   "15USD" → USD 15
///   ""      → null (no discount amount; caller should verify a percentage is set instead)
///
/// Supported symbols: $, €, £, ¥, USD, EUR, GBP, JPY (case-insensitive).
/// Any unrecognised symbol defaults to EGP.
/// </summary>
internal static class DiscountAmountParser
{
    private static readonly Dictionary<string, string> _symbolToCurrency =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["$"]   = "USD",
            ["€"]   = "EUR",
            ["£"]   = "GBP",
            ["¥"]   = "JPY",
            ["usd"] = "USD",
            ["eur"] = "EUR",
            ["gbp"] = "GBP",
            ["jpy"] = "JPY",
        };

    /// <summary>
    /// Returns <c>null</c> when the cell is empty (no discount amount defined).
    /// Returns a failed <see cref="Result{Money}"/> on parse errors.
    /// </summary>
    public static Result<Money>? TryParse(string? rawCell)
    {
        if (string.IsNullOrWhiteSpace(rawCell))
            return null;

        var (amount, currency) = ExtractAmountAndCurrency(rawCell.Trim());

        if (amount is null)
            return Result.Failure<Money>(
                Error.Validation("DiscountAmountParser.InvalidAmount",
                    $"Cannot parse discount amount '{rawCell}'."));

        return Money.Create(amount.Value, currency);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static (decimal? amount, string currency) ExtractAmountAndCurrency(string raw)
    {
        // Check known multi-char codes/symbols longest-first to avoid partial matches.
        foreach (var kv in _symbolToCurrency.OrderByDescending(k => k.Key.Length))
        {
            if (raw.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase))
            {
                var rest = raw[kv.Key.Length..].Trim();
                if (decimal.TryParse(rest,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var a))
                    return (a, kv.Value);
            }

            if (raw.EndsWith(kv.Key, StringComparison.OrdinalIgnoreCase))
            {
                var rest = raw[..^kv.Key.Length].Trim();
                if (decimal.TryParse(rest,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var a))
                    return (a, kv.Value);
            }
        }

        // No recognised symbol — treat as EGP.
        if (decimal.TryParse(raw,
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out var plain))
            return (plain, "EGP");

        return (null, "EGP");
    }
}
