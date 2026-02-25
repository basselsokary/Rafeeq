using Domain.Common;
using Domain.Exceptions;
using Domain.Enums;

namespace Domain.Entities.SiteAggregate;

public class SiteFacility : BaseAuditableEntity
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public FacilityType Type { get; set; }
    public bool IsAvailable { get; private set; }

    private SiteFacility() { }
    private SiteFacility(string name, FacilityType type, string? description)
    {
        Name = name;
        Description = description;
        IsAvailable = true;
    }

    public static SiteFacility Create(string name, FacilityType type, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleValidationException("Facility name cannot be empty.");

        return new SiteFacility(name.Trim(), type, description?.Trim());
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
