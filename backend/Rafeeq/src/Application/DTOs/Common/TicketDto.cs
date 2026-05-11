namespace Application.DTOs.Common;

public sealed record TicketDto(
    MoneyDto? EgyptianTicketPrice,
    MoneyDto? ForeingerTicketPrice,
    string? Notes);
