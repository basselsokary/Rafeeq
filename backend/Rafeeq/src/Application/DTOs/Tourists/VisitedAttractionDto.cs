namespace Application.DTOs.Tourists;

/// <summary>
/// Visited Site DTO
/// </summary>
public record VisitedSiteDto(
    Guid Id,
    Guid SiteId,
    string SiteName,
    string SiteType,
    string? SiteImageUrl,
    string City,
    DateTime VisitDate,
    int DurationMinutes,
    string? Notes,
    bool HasReviewed);
