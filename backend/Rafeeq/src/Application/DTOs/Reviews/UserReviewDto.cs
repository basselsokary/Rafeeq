namespace Application.DTOs.Reviews;

/// <summary>
/// User review DTO (reviews by specific user)
/// </summary>
public record UserReviewDto(
    Guid Id,
    Guid SiteId,
    string SiteName,
    string? SiteImageUrl,
    string SiteType,
    int Rating,
    string Title,
    string Status,
    int HelpfulCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
