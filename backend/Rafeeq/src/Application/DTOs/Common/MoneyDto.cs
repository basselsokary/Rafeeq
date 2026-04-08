namespace Application.DTOs.Common;

/// <summary>
/// Represents a monetary value
/// </summary>
public record MoneyDto(
    decimal Amount,
    string Currency,
    string FormattedAmount); // "200.00 EGP"
