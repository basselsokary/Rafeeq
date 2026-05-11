using Application.DTOs.Common;
using Domain.Enums;

namespace Application.DTOs.Sites;

public record SiteDetailDto(
    Guid Id,
    string CityName,
    string Name,
    string Description,
    SiteType Type,
    SiteStatus Status,
    LocationDto Location,
    string Address,
    string? ContactPhone,
    string? Website,
    string? MainImageUrl,
    double AverageRating,
    int TotalRating,
    TicketDto? EntryTicket,
    List<ImageDto> Images,
    List<OpeningHourDto> OpeningHours,
    List<FacilityType> FacilityTypes,
    List<NearestTransportationDto> NearestTransportations,
    bool IsFree,
    bool IsFeatured,
    List<string> FacilityTypeDisplays,
    string TypeDisplay = "",
    string StatusDisplay = "");