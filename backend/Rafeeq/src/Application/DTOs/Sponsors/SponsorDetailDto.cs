using Application.DTOs.Common;
using Domain.Enums;

namespace Application.DTOs.Sponsors;

public record SponsorDetailDto(
    Guid Id,
    string Name,
    string Description,
    SponsorType Type,
    LocationDto Location,
    string Address,
    string? ContactPhone,
    string ContactEmail,
    string? Website,
    double AverageRating,
    int TotalReviews,
    List<ImageDto> Images,
    List<SponsorOfferDto> ActiveOffers,
    DateRangeDto DateRange,
    bool IsContractValid,
    SponsorStatus Status,
    int TotalClicks,
    int TotalRedemptions,
    double? DistanceKm,
    string TypeDisplay = "");
