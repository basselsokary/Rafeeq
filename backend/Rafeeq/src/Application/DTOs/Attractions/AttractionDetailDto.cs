using Application.DTOs.Common;

namespace Application.DTOs.Attractions;

public record AttractionDetailDto(
    Guid Id,
    string Name,
    string Description,
    string Type,
    string Status,
    LocationDto Location,
    AddressDto Address,
    string? ContactPhone,
    string? ContactEmail,
    string? Website,
    double AverageRating,
    int TotalReviews,
    int TotalVisits,
    List<ImageDto> Images,
    string? AudioGuideUrl,
    string? VirtualTourUrl,
    MoneyDto? EntryFee,
    int EstimatedDurationMinutes,
    string Accessibility,
    LocalizedContentDto? LocalizedContent,
    bool IsFeatured,
    bool IsPopular,
    double? DistanceKm,
    bool IsCurrentlyOpen,
    TimeSpan? TimeUntilClosing,
    string? NextOpeningTime,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
