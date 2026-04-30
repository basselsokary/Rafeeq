using Domain.Common;
using Domain.Enums;
using Shared;

namespace Domain.Entities.AttractionAggregate;

public class AttractionLocalizedContent : BaseAuditableEntity
{
    public LanguageCode Language { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string? LocationDescription { get; private set; }

    private AttractionLocalizedContent() { }
    private AttractionLocalizedContent(LanguageCode language, string name, string description, string? locationDescription)
    {
        Language = language;
        Name = name;
        Description = description;

        LocationDescription = locationDescription;
    }

    internal static Result<AttractionLocalizedContent> Create(LanguageCode language, string name, string description, string? locationDescription)
    {    
        if (string.IsNullOrWhiteSpace(name))
            return AttractionErrors.NameRequired;
        
        if (string.IsNullOrWhiteSpace(description))
            return AttractionErrors.DescriptionRequired;

        return new AttractionLocalizedContent(language, name.Trim(), description.Trim(), locationDescription?.Trim());
    }

    internal Result Update(string name, string description, string? locationDescription)
    {
        if (string.IsNullOrWhiteSpace(name))
            return AttractionErrors.NameRequired;
        
        if (string.IsNullOrWhiteSpace(description))
            return AttractionErrors.DescriptionRequired;

        Name = name.Trim();
        Description = description.Trim();
        LocationDescription = locationDescription?.Trim();

        return Result.Success();
    }
}
