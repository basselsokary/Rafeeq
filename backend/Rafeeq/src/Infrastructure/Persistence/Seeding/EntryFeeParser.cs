using Domain.ValueObjects;
using Shared;

namespace Infrastructure.Persistence.Seeding;

/// <summary>
/// Parses the CSV entry-fee cell into a <see cref="Ticket"/> value object.
///
/// CSV column: <c>Entry Fee 'NULLABLE'</c>
/// Format: two values separated by a comma — <c>foreignAdult,localAdult</c>
///
/// Rules
/// ─────
/// • The cell is nullable. An empty or whitespace-only cell means no ticket at all
///   (do not call <see cref="Site.SetEntryFee"/>; rely on <c>Is Free?</c> instead).
/// • Local price is always EGP. It is never written explicitly in the file.
/// • Foreign price may carry an explicit currency symbol / code prefix:
///     - "8$"  → USD 8
///     - "8€"  → EUR 8
///     - "8"   → USD 8  (fallback when no symbol is present)
///   Supported prefix/suffix symbols: $, €, £, ¥, USD, EUR, GBP, JPY (case-insensitive).
///   Any unrecognised symbol is treated as USD.
/// • Either price segment may be "0" (free for that category).
///
/// Examples
/// ────────
///   "100,50"   → EGP 50 (local),  USD 100 (foreigner)
///   "8$,60"    → EGP 60 (local),  USD 8   (foreigner)
///   "0,0"      → both free  → caller should use RemoveEntryFee(isFree:true) instead
///   ""         → null        → no ticket
/// </summary>
internal static class EntryFeeParser
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
    /// Attempts to parse the raw CSV cell value into a <see cref="Ticket"/>.
    /// Returns <c>null</c> when the cell is empty (no ticket defined).
    /// </summary>
    public static Result<Ticket>? TryParse(string? rawCell)
    {
        if (string.IsNullOrWhiteSpace(rawCell) || rawCell.Trim().Equals("NULL", StringComparison.OrdinalIgnoreCase))
            return null; // No ticket defined — caller decides what to do.

        var parts = rawCell.Split(',');
        if (parts.Length != 2)
            return Result.Failure<Ticket>(
                Error.Validation("EntryFeeParser.InvalidFormat",
                    $"Expected 'foreignAdult,localAdult' but got '{rawCell}'."));

        var (foreignRaw, localRaw) = (parts[0].Trim(), parts[1].Trim());

        // --- Local price (always EGP) ---
        if (!TryParseAmount(localRaw, out var localAmount))
            return Result.Failure<Ticket>(
                Error.Validation("EntryFeeParser.InvalidLocalAmount",
                    $"Cannot parse local amount '{localRaw}'."));

        var localMoneyResult = Money.Create(localAmount, "EGP");
        if (localMoneyResult.Failed)
            return Result.Failure<Ticket>(localMoneyResult.Error);

        // --- Foreign price ---
        if (!TryParseAmountWithCurrency(foreignRaw, out var foreignAmount, out var foreignCurrency))
            return Result.Failure<Ticket>(
                Error.Validation("EntryFeeParser.InvalidForeignAmount",
                    $"Cannot parse foreign amount '{foreignRaw}'."));

        var foreignMoneyResult = Money.Create(foreignAmount, foreignCurrency);
        if (foreignMoneyResult.Failed)
            return Result.Failure<Ticket>(foreignMoneyResult.Error);

        return Ticket.Create(localMoneyResult.Value, foreignMoneyResult.Value);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static bool TryParseAmount(string raw, out decimal amount)
    {
        // Strip any trailing/leading currency symbols before parsing.
        var clean = StripCurrencySymbols(raw);
        return decimal.TryParse(clean,
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out amount);
    }

    private static bool TryParseAmountWithCurrency(
        string raw, out decimal amount, out string currency)
    {
        currency = "USD"; // Default foreign currency.
        amount   = 0;

        if (string.IsNullOrWhiteSpace(raw))
            return false;

        // Check for known multi-char codes first (e.g. "USD100", "100USD").
        foreach (var kv in _symbolToCurrency)
        {
            if (raw.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase))
            {
                currency = kv.Value;
                var rest = raw[kv.Key.Length..].Trim();
                return decimal.TryParse(rest,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out amount);
            }

            if (raw.EndsWith(kv.Key, StringComparison.OrdinalIgnoreCase))
            {
                currency = kv.Value;
                var rest = raw[..^kv.Key.Length].Trim();
                return decimal.TryParse(rest,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out amount);
            }
        }

        // No symbol found — treat as plain number with default USD.
        return decimal.TryParse(raw,
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out amount);
    }

    private static string StripCurrencySymbols(string raw)
    {
        foreach (var symbol in _symbolToCurrency.Keys)
        {
            if (raw.StartsWith(symbol, StringComparison.OrdinalIgnoreCase))
                return raw[symbol.Length..].Trim();

            if (raw.EndsWith(symbol, StringComparison.OrdinalIgnoreCase))
                return raw[..^symbol.Length].Trim();
        }
        return raw;
    }
}
