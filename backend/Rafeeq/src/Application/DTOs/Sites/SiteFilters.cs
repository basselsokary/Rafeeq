using Domain.Enums;

namespace Application.DTOs.Sites;

public record SiteFilters(
    SiteType? Type,
    Guid? City,
    bool? IsFree,
    int? MinRating,
    int? MaxRating);
