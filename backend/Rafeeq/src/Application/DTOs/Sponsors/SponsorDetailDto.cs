using Application.DTOs.Common;

namespace Application.DTOs.Sponsors;

/// <summary>
/// Sponsor detail DTO
/// </summary>
public record SponsorDetailDto(
    Guid Id,
    string Name,
    string Description,
    string Type,
    string Tier,
    LocationDto Location,
    AddressDto Address,
    string ContactPhone,
    string ContactEmail,
    string? Website,
    double AverageRating,
    int TotalReviews,
    List<ImageDto> Images,
    List<SponsorOfferDto> ActiveOffers,
    DateTime ContractStartDate,
    DateTime ContractEndDate,
    bool IsContractValid,
    bool IsActive,
    int TotalClicks,
    int TotalRedemptions,
    double? DistanceKm,
    DateTime CreatedAt);
