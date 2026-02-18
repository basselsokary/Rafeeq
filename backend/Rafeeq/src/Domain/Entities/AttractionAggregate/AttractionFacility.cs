using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Enums;

namespace Domain.Entities.AttractionAggregate;

public class AttractionFacility : BaseAuditableEntity
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public FacilityType Type { get; set; }
    public bool IsAvailable { get; private set; }

    private AttractionFacility() { }
    private AttractionFacility(string name, FacilityType type, string? description)
    {
        Name = name;
        Description = description;
        IsAvailable = true;
    }

    public static AttractionFacility Create(string name, FacilityType type, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleValidationException("Facility name cannot be empty.");

        return new AttractionFacility(name.Trim(), type, description?.Trim());
    }

    public void Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleValidationException("Facility name cannot be empty.");

        Name = name.Trim();
        Description = description?.Trim();
        MarkAsUpdated();
    }

    public void SetAvailability(bool isAvailable)
    {
        IsAvailable = isAvailable;
        MarkAsUpdated();
    }
}
