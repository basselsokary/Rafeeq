using Application.DTOs.Common;
using Domain.Entities.SponsorAggregate;
using Domain.Enums;

namespace Application.DTOs.Admins;

public sealed record AdminSponsorDetailDto(
    Guid Id,
    string Title,
    string Description,
    SponsorType Type,
    SponsorTier Tier,
    LocationDto Location,
    string Address,
    string? ContactPhone,
    string? ContactEmail,
    string? WebsiteUrl,
    string? MainImageUrl, 
    DateRangeDto DateRange,
    bool IsContractValid,
    SponsorStatus Status,
    int TotalRedemptions,
    DateTime CreatedAt,
    Guid CreatedBy,
    string CreatedByName,
    DateTime? LastModifiedAt,
    Guid? LastModifiedBy,
    string? LastModifiedByName);

public sealed record AdminSponsorLocalizedContentDto(
    Guid Id,
    LanguageCode Language,
    string Title,
    string Description,
    string Address,
    AuditInfoDto AuditInfo);

public sealed record AdminSponsorDashboardDto(
    int TotalSponsors,
    int ActiveSponsors,
    int ExpiredSponsors,
    int TotalOffers,
    int ActiveOffers);