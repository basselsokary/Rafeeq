namespace Application.DTOs.Attractions;

/// <summary>
/// Localized content DTO
/// </summary>
public record LocalizedContentDto(
    string Language,
    string Name,
    string Description);
