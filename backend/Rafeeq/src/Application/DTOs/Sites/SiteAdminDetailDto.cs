using Application.DTOs.Common;

namespace Application.DTOs.Sites;

public record SiteAdminDetailDto(
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
    bool IsFree,
    bool IsFeatured,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
