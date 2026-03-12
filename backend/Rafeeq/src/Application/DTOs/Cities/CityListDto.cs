namespace Application.DTOs.Cities;

public record CityListDto(
    string Name,
    string Description,
    string? ImageUrl,
    int TotalSites,
    int DisplayOrder);
