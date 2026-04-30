using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;
using Shared;

namespace Domain.Entities.SiteAggregate;

public class NearestTransportationLocalizedContent : BaseAuditableEntity
{
    public LanguageCode Language { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public Address? Address { get; private set; }

    private NearestTransportationLocalizedContent() { }
    private NearestTransportationLocalizedContent(LanguageCode language, string name, string? description, Address? address)
    {
        Language = language;
        Name = name;
        Description = description;
        
        Address = address;
    }

    internal static Result<NearestTransportationLocalizedContent> Create(
        LanguageCode language,
        string name,
        string? description,
        Address? address)
    {
        if (string.IsNullOrWhiteSpace(name))
            return SiteErrors.NearestTransportationNameRequired;

        return new NearestTransportationLocalizedContent(language, name.Trim(), description?.Trim(), address);
    }

    internal Result Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return SiteErrors.NearestTransportationNameRequired;

        Name = name.Trim();
        Description = description?.Trim();

        return Result.Success();
    }

    internal void UpdateAddress(Address address)
    {
        if (Address != address)
            Address = address;
    }
}
