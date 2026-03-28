using Application.DTOs.Common;

namespace Application.DTOs.Attractions;

public record AttractionListDto(
    Guid Id,
    string Name,
    string ShortDescription,
    string Type,
    LocationDto? Location,
    string? PrimaryImageUrl);
