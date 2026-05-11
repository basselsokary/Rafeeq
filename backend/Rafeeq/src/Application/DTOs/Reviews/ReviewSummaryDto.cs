namespace Application.DTOs.Reviews;

public record ReviewSummaryDto(
    Guid Id,
    string UserName,
    int Rating,
    string Title,
    string ShortContent,
    int HelpfulCount);
