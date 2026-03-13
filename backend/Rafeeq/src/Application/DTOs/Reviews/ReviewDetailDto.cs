namespace Application.DTOs.Reviews;

/// <summary>
/// Detailed review DTO
/// </summary>
public record ReviewDetailDto(
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
    string? RejectionReason,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
