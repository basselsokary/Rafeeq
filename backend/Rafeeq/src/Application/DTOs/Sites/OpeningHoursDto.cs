namespace Application.DTOs.Sites;

/// <summary>
/// Opening hours DTO
/// </summary>
public record OpeningHoursDto(
    string Day,
    string? OpenTime,
    string? CloseTime,
    bool IsClosed);
