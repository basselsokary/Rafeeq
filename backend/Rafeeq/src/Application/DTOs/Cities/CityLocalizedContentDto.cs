using Domain.Enums;

namespace Application.DTOs.Cities;

public record CityLocalizedContentDto(
    Guid ContentId,
    LanguageCode Language,
    string Name,
    string Description);
