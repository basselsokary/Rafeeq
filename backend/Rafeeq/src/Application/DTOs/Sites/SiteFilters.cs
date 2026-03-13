using Domain.Enums;

namespace Application.DTOs.Sites;

public record SiteFilters(
    List<SiteType>? Types,
    Guid? City,
    bool? IsFree,
    int? MinRating,
    int? MaxRating);
