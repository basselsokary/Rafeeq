using Application.DTOs.Common;

namespace Application.DTOs.Cities;

public record CityListDto(
    Guid Id,
    string Name,
    string Description,
    LocationDto CenterLocation,
    string? ImageUrl,
    int TotalSites);
