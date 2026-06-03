using Application.DTOs.Common;
using Domain.Enums;

namespace Application.DTOs.Trips;

public sealed record TripListDto(
    Guid Id,
    string Title,
    string Description,
    DateOnly StartDate,
    DateOnly EndDate,
    TripStatus Status,
    int TotalDays,
    int TotalSites,
    int EstimatedTotalDurationMinutes,
    MoneyDto? EstimatedTotalBudget,
    MoneyDto? ActualCost,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string StatusDisplay = "");
