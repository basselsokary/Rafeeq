using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities.AttractionAggregate;

public class AttractionLocalizedContent : BaseAuditableEntity
{
    public LanguageCode Language { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    private AttractionLocalizedContent() { }

    private AttractionLocalizedContent(LanguageCode language, string name, string description)
    {
        Language = language;
        Name = name;
        Description = description;
    }

    public static AttractionLocalizedContent Create(LanguageCode language, string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleValidationException("Name cannot be empty.");

        if (string.IsNullOrWhiteSpace(description))
            throw new BusinessRuleValidationException("Description cannot be empty.");

        return new AttractionLocalizedContent(language, name.Trim(), description.Trim());
    }

    public void Update(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleValidationException("Name cannot be empty.");

        if (string.IsNullOrWhiteSpace(description))
            throw new BusinessRuleValidationException("Description cannot be empty.");

        Name = name.Trim();
        Description = description.Trim();
        MarkAsUpdated();
    }
}
