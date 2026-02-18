using Domain.Common;
using Domain.Common.Exceptions;

namespace Domain.ValueObjects;

public class GeoLocation : ValueObject
{
    public double Latitude { get; }
    public double Longitude { get; }
    
    private GeoLocation() { }
    private GeoLocation(double lat, double lng)
    {
        Latitude = lat;
        Longitude = lng;
    }

    public static GeoLocation Create(double latitude, double longitude)
    {        
        if (latitude < -90 || latitude > 90)
            throw new BusinessRuleValidationException("Latitude must be between -90 and 90 degrees.");

        if (longitude < -180 || longitude > 180)
            throw new BusinessRuleValidationException("Longitude must be between -180 and 180 degrees.");

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

