namespace Application.DTOs.Users;

public record LoginHistoryDto(
    Guid Id,
    DateTime LoginAt,
    string? IpAddress,
    string? UserAgent,
    string? Device,  // Mobile, Desktop, Tablet
    string? Browser,
    string? Location,  // City, Country
    bool WasSuccessful,
    string? FailureReason);
