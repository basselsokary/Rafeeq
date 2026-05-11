using Domain.Enums;

namespace Application.DTOs.Reviews;

public record ReviewListDto(
    Guid Id,
    Guid SiteId,
    string SiteName,
    Guid UserId,
    string UserName,
    int Rating,
    string Title,
    string Content,
    ReviewStatus Status,
    int HelpfulCount,
    int NotHelpfulCount,
    double HelpfulnessScore,
    DateTime CreatedAt,
    string StatusDisplay = "");
