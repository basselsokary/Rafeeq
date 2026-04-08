namespace Application.DTOs.Reviews;

/// <summary>
/// Review summary for cards
/// </summary>
public record ReviewSummaryDto(
    Guid Id,
    string UserName,
    int Rating,
    string Title,
    string ShortContent,
    int HelpfulCount);
