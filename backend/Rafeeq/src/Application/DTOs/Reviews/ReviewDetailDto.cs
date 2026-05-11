using Domain.Enums;

namespace Application.DTOs.Reviews;

public record ReviewDetailDto(
    Guid Id,
    Guid SiteId,
    string SiteName,
    SiteType SiteType,
    Guid UserId,
    string UserName,
    int Rating,
    string Title,
    string Content,
    ReviewStatus Status,
    int HelpfulCount,
    int NotHelpfulCount,
    double HelpfulnessScore,
    string? RejectionReason,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string SiteTypeDisplay = "",
    string StatusDisplay = "");
