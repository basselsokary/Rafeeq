using Domain.Enums;

namespace Application.DTOs.Reviews;

public record TouristReviewDto(
    Guid Id,
    Guid SiteId,
    string SiteName,
    string? SiteImageUrl,
    SiteType SiteType,
    int Rating,
    string Title,
    ReviewStatus Status,
    int HelpfulCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string SiteTypeDisplay = "",
    string StatusDisplay = "");
