namespace API.DTOs;

public record TicketRequest(
    decimal? EgyptianTicketPrice,
    decimal? ForeignerTicketPrice,
    string? ForeignerCurrency,
    string? Notes,
    bool IsFree);
