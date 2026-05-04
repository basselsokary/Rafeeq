using Domain.Common;
using Domain.Enums;
using Shared;

namespace Domain.Entities.CityAggregate;

public class CityLocalizedContent : BaseAuditableEntity
{
    public LanguageCode Language { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    private CityLocalizedContent() { }
    private CityLocalizedContent(LanguageCode language, string name, string description)
    {
        Language = language;
        Name = name;
        Description = description;
    }

    internal static Result<CityLocalizedContent> Create(LanguageCode language, string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return CityErrors.NameRequired;
        
        if (string.IsNullOrWhiteSpace(description))
            return CityErrors.DescriptionRequired;

        return new CityLocalizedContent(language, name.Trim(), description.Trim());
    }

    internal Result<CityLocalizedContent> Update(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return CityErrors.NameRequired;
        
        if (string.IsNullOrWhiteSpace(description))
            return CityErrors.DescriptionRequired;

        Name = name.Trim();
        Description = description.Trim();

        return this;
    }
}
