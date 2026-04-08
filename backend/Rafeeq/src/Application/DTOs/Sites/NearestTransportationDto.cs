using Application.DTOs.Common;

namespace Application.DTOs.Sites;

public record NearestTransportationDto(
    Guid Id,
    string Type,
    string Name,
    LocationDto Location,
    AddressDto? Address,
    string? Description,
    bool IsOperational,
    bool HasAccessibility,
    TimeRangeDto? OperatingHours
);