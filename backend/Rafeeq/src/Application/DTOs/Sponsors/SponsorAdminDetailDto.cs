using Application.DTOs.Common;

namespace Application.DTOs.Sponsors;

public record SponsorAdminDetailDto(
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
    DateTime ContractStartDate,
    DateTime ContractEndDate,
    bool IsContractValid,
    bool IsActive,
    int TotalClicks,
    int TotalRedemptions,
    double? DistanceKm,
    DateTime CreatedAt,
    Guid CreatedBy,
    string CreatedByName,
    DateTime? LastModifiedAt,
    Guid? LastModifiedBy,
    string? LastModifiedByName);
