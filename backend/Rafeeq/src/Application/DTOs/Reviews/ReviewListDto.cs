namespace Application.DTOs.Reviews;

/// <summary>
/// Review list DTO
/// </summary>
public record ReviewListDto(
    Guid Id,
    Guid SiteId,
    string SiteName,
    Guid UserId,
    string UserName,
    int Rating,
    string Title,
    string Content,
    string Status,
    int HelpfulCount,
    int NotHelpfulCount,
    double HelpfulnessScore,
    DateTime CreatedAt);
