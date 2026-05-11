namespace Application.DTOs.Common;

public record MoneyDto(
    decimal Amount,
    string Currency,
    string FormattedAmount);
