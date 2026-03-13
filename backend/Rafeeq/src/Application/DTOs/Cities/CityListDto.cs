namespace Application.DTOs.Cities;

public record CityListDto(
    Guid Id,
    string Name,
    string Description,
    string? ImageUrl,
    int TotalSites);
