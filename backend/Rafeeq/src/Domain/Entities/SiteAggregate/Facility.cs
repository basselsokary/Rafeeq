using Domain.Common;
using Shared.Models;

namespace Domain.Entities.SiteAggregate;

public class Facility : BaseAuditableEntity
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public bool IsAvailable { get; private set; }

    private Facility() { }
    private Facility(string name, string description)
    {
        Name = name;
        Description = description;
        
        IsAvailable = true;
    }

    public static Result<Facility> Create(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return SiteErrors.NameRequired;
        
        if (string.IsNullOrWhiteSpace(name))
            return SiteErrors.DescriptionRequired;

        return new Facility(name.Trim(), description.Trim());
    }

    public Result Update(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return SiteErrors.NameRequired;
        
        if (string.IsNullOrWhiteSpace(name))
            return SiteErrors.DescriptionRequired;

        Name = name.Trim();
        Description = description.Trim();
        MarkAsUpdated();

        return Result.Success();
    }

    public void SetAvailability(bool isAvailable)
    {
        IsAvailable = isAvailable;
        MarkAsUpdated();
    }
}

// site.AddFacility("Parking", "Free parking for 100 cars");
// site.AddFacility("Wheelchair Access", "Ramps available at all entrances");
// site.AddFacility("Restrooms", "Located on ground floor");
// site.AddFacility("Café", "Open 9 AM - 5 PM");
// site.AddFacility("Gift Shop");
// site.AddFacility("Audio Guide", "Available in 5 languages");
// site.AddFacility("WiFi");