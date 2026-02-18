using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Common.Interfaces;
using Domain.ValueObjects;

namespace Domain.Enums;

public class City : BaseAuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public GeoLocation Location { get; private set; } = null!;
    
    private City() { }
    private City(string name, string description, GeoLocation location)
    {
        Name = name;
        Location = location;
        Description = description;
    }

    public static City Create(string name, string description, GeoLocation location)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleValidationException("City name cannot be null or empty.");

        return new City(name, description, location);
    }
}