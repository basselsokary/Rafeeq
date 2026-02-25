using Domain.Common;
using Domain.Exceptions;
using Domain.Enums;

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

    internal static CityLocalizedContent Create(LanguageCode language, string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleValidationException("Localized name cannot be empty.");
        
        if (string.IsNullOrWhiteSpace(description))
            throw new BusinessRuleValidationException("Description cannot be empty.");

        return new CityLocalizedContent(language, name.Trim(), description.Trim());
    }

    internal void Update(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleValidationException("Localized name cannot be empty.");
        
        if (string.IsNullOrWhiteSpace(description))
            throw new BusinessRuleValidationException("Description cannot be empty.");

        Name = name.Trim();
        Description = description.Trim();
        MarkAsUpdated();
    }
}
