using Application.DTOs.Common;

namespace Application.DTOs.Admins;

public sealed record AdminCityDashboardDto(
    int TotalCities,
    int TotalSites);

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