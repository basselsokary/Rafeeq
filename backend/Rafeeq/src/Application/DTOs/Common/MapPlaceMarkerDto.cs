namespace Application.DTOs.Common;

public record MapPlaceMarkerDto(
    Guid Id,
    string Name,
    LocationDto Location,
    string Type,
    string? ImageUrl
);