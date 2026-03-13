using Application.DTOs.Common;

namespace Application.DTOs.Cities;

public record CityDetailDto(
    string Name,
    string Description,
    LocationDto CenterLocation,
    string? ImageUrl,
    int TotalSites,
    int DisplayOrder);
