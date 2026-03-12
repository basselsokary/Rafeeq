namespace Application.DTOs.Common;

/// <summary>
/// Bounding box for map queries
/// </summary>
public record BoundingBox(
    double NorthLatitude,
    double SouthLatitude,
    double EastLongitude,
    double WestLongitude);
