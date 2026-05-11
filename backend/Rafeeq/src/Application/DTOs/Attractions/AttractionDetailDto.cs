using Application.DTOs.Common;
using Domain.Enums;

namespace Application.DTOs.Attractions;

public record AttractionDetailDto(
    Guid Id,
    string Name,
    string Description,
    AttractionType Type,
    LocationDto? Location,
    List<HistoricalPeriod> HistoricalPeriods,
    string? LocationDescription,
    List<ImageDto> Images,
    List<string> HistoricalPeriodDisplay,
    string TypeDisplay = "");
