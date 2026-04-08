using Application.DTOs.Common;

namespace Application.DTOs.Cities;

public record CityAdminDetailDto(
    Guid Id,
    string Name,
    string Description,
    LocationDto CenterLocation,
    string? ImageUrl,
    int TotalSites,
    int DisplayOrder,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
