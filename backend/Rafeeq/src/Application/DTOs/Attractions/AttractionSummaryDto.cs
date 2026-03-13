namespace Application.DTOs.Attractions;

/// <summary>
/// Summary DTO for cards/tiles
/// </summary>
public record AttractionSummaryDto(
    Guid Id,
    string Name,
    string Type,
    string City,
    string? PrimaryImageUrl,
    double AverageRating,
    int TotalReviews,
    bool IsFree,
    int EstimatedDurationMinutes);
