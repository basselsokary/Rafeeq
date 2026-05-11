namespace Application.DTOs.Sites;

public record FacilityDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsAvailable);
