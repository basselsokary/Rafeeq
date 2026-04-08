using Application.DTOs.Common;
using Domain.Enums;

namespace Application.DTOs.Attractions;

public record AttractionAdminDetailDto(
    Guid Id,
    string Name,
    string Description,
    string Type,
    LocationDto? Location,
    HistoricalPeriod HistoricalPeriod,
    string? LocationDescription,
    List<ImageDto> Images,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
