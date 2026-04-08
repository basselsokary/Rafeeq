using Domain.Common;
using Domain.Enums;
using Shared.Models;

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

    internal Result Update(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return CityErrors.NameRequired;
        
        if (string.IsNullOrWhiteSpace(description))
            return CityErrors.DescriptionRequired;

        Name = name.Trim();
        Description = description.Trim();
        MarkAsUpdated();

        return Result.Success();
    }
}
