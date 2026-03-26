using Domain.Common;
using Shared.Models;

namespace Domain.ValueObjects;

public class GeoLocation : ValueObject
{
    public const int BoundLatitude = 90;
    public const int BoundLongitude = 180;
    
    public double Latitude { get; }
    public double Longitude { get; }
    
    private GeoLocation() { }
    private GeoLocation(double lat, double lng)
    {
        Latitude = lat;
        Longitude = lng;
    }

    public static Result<GeoLocation> Create(double latitude, double longitude)
    {
        if (latitude < -BoundLatitude || latitude > BoundLatitude)
            return GeoLocationErrors.InvalidLatitude(BoundLatitude);

        if (longitude < -BoundLongitude || longitude > BoundLongitude)
            return GeoLocationErrors.InvalidLongitude(BoundLongitude);

        return new GeoLocation(latitude, longitude);
    }

    /// <summary>
    /// Calculate distance to another location using Haversine formula (in kilometers)
    /// </summary>
    public double DistanceTo(GeoLocation other)
    {
        const double earthRadiusKm = 6371;

        var dLat = DegreesToRadians(other.Latitude - Latitude);
        var dLon = DegreesToRadians(other.Longitude - Longitude);

        var thisLatitude = DegreesToRadians(Latitude);
        var otherLatitude = DegreesToRadians(other.Latitude);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(thisLatitude) * Math.Cos(otherLatitude);
        
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Latitude;
        yield return Longitude;
    }
}

