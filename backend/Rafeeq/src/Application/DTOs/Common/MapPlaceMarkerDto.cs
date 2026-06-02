namespace Application.DTOs.Common;

public record MapPlaceMarkerDto(
    Guid Id,
    string Name,
    LocationDto Location,
    MarkerType Type,
    string? ImageUrl,
    string TypeDisplay = "");

public enum MarkerType
{
    Site,
    Sponsor
}