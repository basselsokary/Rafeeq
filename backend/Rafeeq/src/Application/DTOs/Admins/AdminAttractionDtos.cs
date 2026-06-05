using Application.DTOs.Common;
using Domain.Enums;

namespace Application.DTOs.Admins;

public sealed record AdminAttractionDashboardDto(
    int TotalAttractions,
    int FeaturedAttractions);

public record AttractionAdminDetailDto(
    Guid Id,
    string Name,
    string Description,
    AttractionType Type,
    LocationDto? Location,
    List<HistoricalPeriod> HistoricalPeriods,
    string? LocationDescription,
    bool IsFeatured,
    List<ImageDto> Images,
    DateTime CreatedAt,
    Guid CreatedBy,
    DateTime? LastModifiedAt,
    Guid? LastModifiedBy);