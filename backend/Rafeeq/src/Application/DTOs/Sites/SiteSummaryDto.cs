namespace Application.DTOs.Sites;

/// <summary>
/// Summary DTO for cards/tiles
/// </summary>
public record SiteSummaryDto(
    Guid Id,
    string Name,
    string Type,
    string City,
    string? PrimaryImageUrl,
    double AverageRating,
    int TotalReviews,
    bool IsFree);
