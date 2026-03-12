using Application.DTOs.Common;

namespace Application.DTOs.Sites;

public record NearestTransportationDto(
    string Type,
    string Name,
    double DistanceKm,
    LocationDto Location,
    AddressDto? Address,
    string? Description,
    bool IsOperational,
    bool HasAccessibility,
    TimeRangeDto? OperatingHours
);