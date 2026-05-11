using Domain.Enums;

namespace Application.DTOs.Attractions;

public record AttractionLocalizedContentDto(
    Guid Id,
    LanguageCode Language,
    string Name,
    string Description,
    string? LocationDescription);
