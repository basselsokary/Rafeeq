using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;
using Shared.Models;

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

    internal static Result<AttractionLocalizedContent> Create(LanguageCode language, string name, string description)
    {    
        if (string.IsNullOrWhiteSpace(name))
            return AttractionErrors.NameRequired;
        
        if (string.IsNullOrWhiteSpace(description))
            return AttractionErrors.DescriptionRequired;

        return new AttractionLocalizedContent(language, name.Trim(), description.Trim());
    }

    internal Result Update(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return AttractionErrors.NameRequired;
        
        if (string.IsNullOrWhiteSpace(description))
            return AttractionErrors.DescriptionRequired;

        Name = name.Trim();
        Description = description.Trim();
        MarkAsUpdated();

        return Result.Success();
    }
}
