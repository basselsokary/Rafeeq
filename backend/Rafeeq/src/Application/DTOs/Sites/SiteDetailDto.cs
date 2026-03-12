using Application.DTOs.Common;

namespace Application.DTOs.Sites;

public record SiteDetailDto(
    Guid Id,
    string Name,
    string Description,
    string Type,
    string Status,
    LocationDto Location,
    AddressDto Address,
    string? ContactPhone,
    string? Website,
    double AverageRating,
    int TotalReviews,
    MoneyDto? EntryFee,
    List<ImageDto> Images,
    List<OpeningHoursDto> OpeningHours,
    List<FacilityDto> Facilities,
    List<NearestTransportationDto> NearestTransportations,
    bool IsFeatured,
    bool IsCurrentlyOpen,
    string? NextOpeningTime);
