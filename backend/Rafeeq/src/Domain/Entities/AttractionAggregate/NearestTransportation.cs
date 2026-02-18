using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities.AttractionAggregate;

public class NearestTransportation : BaseAuditableEntity
{
    public string Name { get; private set; } = null!;
    public TransportType Type { get; private set; }
    public GeoLocation Location { get; private set; } = null!;
    public Address? Address { get; private set; }
    public string? Description { get; private set; }
    public bool IsOperational { get; private set; }
    public bool HasAccessibility { get; private set; }
    public TimeRange? OperatingHours { get; private set; }

    private NearestTransportation() { }

    private NearestTransportation(
        string name,
        TransportType type,
        GeoLocation location,
        Address address,
        string? description)
    {
        Name = name;
        Type = type;
        Location = location;
        Description = description;
        Address = address;

        IsOperational = false;
        HasAccessibility = false;
    }

    public static NearestTransportation Create(
        string name,
        TransportType type,
        GeoLocation location,
        Address address,
        string? description,
        TimeRange? operatingHours)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleValidationException("Transport station name cannot be empty.");

        return new NearestTransportation(
            name.Trim(),
            type,
            location,
            address,
            description);
    }

    public void UpdateDetails(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleValidationException("Transport station name cannot be empty.");

        Name = name.Trim();
        Description = description?.Trim();

        MarkAsUpdated();
    }

    public void UpdateAddress(Address address)
    {
        Address = address;
        MarkAsUpdated();
    }

    public void SetOperatingHours(TimeRange operatingHours)
    {
        OperatingHours = operatingHours;
        MarkAsUpdated();
    }

    public void SetAccessibility(bool hasAccessibility)
    {
        HasAccessibility = hasAccessibility;
        MarkAsUpdated();
    }

    public void SetOperationalStatus(bool isOperational)
    {
        IsOperational = isOperational;
        MarkAsUpdated();
    }
}
