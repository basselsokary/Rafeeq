namespace Application.DTOs.Sites;

/// <summary>
/// Facility DTO
/// </summary>
public record FacilityDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsAvailable);
