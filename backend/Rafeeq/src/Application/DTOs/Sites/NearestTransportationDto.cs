using Application.DTOs.Common;
using Domain.Enums;

namespace Application.DTOs.Sites;

public record NearestTransportationDto(
    Guid Id,
    TransportationType Type,
    string Name,
    string? Description,
    LocationDto Location,
    string? Address,
    double DistanceKm,
    bool IsOperational,
    bool HasAccessibility,
    TimeRangeDto? OperatingHours,
    string TypeDisplay = "");
    