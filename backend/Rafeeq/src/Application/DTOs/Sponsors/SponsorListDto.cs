using Domain.Enums;

namespace Application.DTOs.Sponsors;

public record SponsorListDto(
    Guid Id,
    string Title,
    SponsorType Type,
    double Latitude,
    double Longitude,
    string? PrimaryImageUrl,
    bool IsContractValid,
    SponsorStatus Status,
    int ActiveOffersCount,
    double? DistanceKm,
    string TypeDisplay = "");
