namespace Application.DTOs.Attractions;

public record LocalizedContentDto(
    string Language,
    string Name,
    string Description);
