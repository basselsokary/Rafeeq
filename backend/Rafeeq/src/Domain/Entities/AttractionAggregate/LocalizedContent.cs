using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Enums;

namespace Domain.Entities.AttractionAggregate;

public class LocalizedContent : BaseAuditableEntity
{
    public LanguageCode Language { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string? HistoricalContext { get; private set; }
    public string? VisitorTips { get; private set; }

    private LocalizedContent() { }

    private LocalizedContent(LanguageCode language, string name, string description)
    {
        Language = language;
        Name = name;
        Description = description;
    }

    public static LocalizedContent Create(LanguageCode language, string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleValidationException("Name cannot be empty.");

        if (string.IsNullOrWhiteSpace(description))
            throw new BusinessRuleValidationException("Description cannot be empty.");

        return new LocalizedContent(language, name.Trim(), description.Trim());
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

    public void SetAdditionalContent(string? historicalContext, string? visitorTips)
    {
        HistoricalContext = historicalContext?.Trim();
        VisitorTips = visitorTips?.Trim();
        MarkAsUpdated();
    }
}
