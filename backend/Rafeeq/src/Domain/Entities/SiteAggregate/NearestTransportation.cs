using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;
using Shared;

namespace Domain.Entities.SiteAggregate;

public class NearestTransportation : BaseAuditableEntity
{
    public Guid SiteId { get; private set; }
    
    public TransportationType Type { get; private set; }
    public GeoLocation Location { get; private set; } = null!;

    public double DistanceKm { get; private set; }
    public bool IsOperational { get; private set; }
    public bool HasAccessibility { get; private set; }
    public TimeRange? OperatingHours { get; private set; }

    private readonly List<NearestTransportationLocalizedContent> _localizedContents = [];
    public IReadOnlyCollection<NearestTransportationLocalizedContent> LocalizedContents => _localizedContents.AsReadOnly();

    private NearestTransportation() { }
    private NearestTransportation(
        TransportationType type,
        GeoLocation location,
        double distanceKm)
    {
        Type = type;
        Location = location;
        DistanceKm = distanceKm;

        IsOperational = false;
        HasAccessibility = false;
    }

    internal static Result<NearestTransportation> Create(
        TransportationType type,
        GeoLocation location,
        double distanceKm)
    {
        var transportation = new NearestTransportation(
            type,
            location,
            distanceKm);

        return Result.Success(transportation);
    }

    public void UpdateLocation(GeoLocation location, double distanceKm)
    {
        Location = location;
        DistanceKm = distanceKm;
    }

    public void SetOperatingHours(TimeRange operatingHours)
    {
        if (operatingHours == OperatingHours)
            return;
    
        OperatingHours = operatingHours;
    }

    public void SetAccessibility(bool hasAccessibility)
    {
        if (hasAccessibility == HasAccessibility)
            return;

        HasAccessibility = hasAccessibility;
    }

    public void SetOperationalStatus(bool isOperational)
    {
        if (isOperational == IsOperational)
            return;

        IsOperational = isOperational;
    }

    public Result AddLocalizedContent(
        LanguageCode language,
        string name,
        string? description,
        Address? address)
    {
        var contentResult = NearestTransportationLocalizedContent.Create(language, name, description, address);
        if (contentResult.Failed)
            return contentResult;

        var existing = _localizedContents.FirstOrDefault(lc => lc.Language == language);
        if (existing != null)
            _localizedContents.Remove(existing);

        _localizedContents.Add(contentResult.Value);

        return Result.Success();
    }

    public Result UpdateLocalizedContent(
        Guid contentId,
        string name,
        string? description,
        Address? address)
    {
        var existing = _localizedContents.FirstOrDefault(lc => lc.Id == contentId);
        if (existing == null)
            return SiteErrors.LocalizedContentNotFound;

        Result result = existing.Update(name, description);
        if (result.Failed)
            return result;

        if (address != null)
            existing.UpdateAddress(address);
        
        return Result.Success();
    }
}
