using Application.DTOs.Common;
using Domain.Enums;

namespace Application.DTOs.Attractions;

public record AttractionListDto(
    Guid Id,
    string Name,
    string Description,
    AttractionType Type,
    LocationDto? Location,
    string? PrimaryImageUrl,
    string TypeDisplay = "");
