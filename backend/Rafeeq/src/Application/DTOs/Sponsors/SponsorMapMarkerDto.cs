using Application.DTOs.Common;

namespace Application.DTOs.Sponsors;

public record SponsorMapMarkerDto(
    Guid Id,
    string Name,
    LocationDto Location,
    string? PrimaryImageUrl
);
