using Domain.Common;
using Domain.Enums;
using Shared.Models;

namespace Domain.Entities.SiteAggregate;

public class SiteLocalizedContent : BaseAuditableEntity
{
    public LanguageCode Language { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    private SiteLocalizedContent() { }
    private SiteLocalizedContent(LanguageCode language, string name, string description)
    {
        Language = language;
        Name = name;
        Description = description;
    }

    internal static Result<SiteLocalizedContent> Create(LanguageCode language, string name, string description)
    {    
        if (string.IsNullOrWhiteSpace(name))
            return SiteErrors.NameRequired;
        
        if (string.IsNullOrWhiteSpace(description))
            return SiteErrors.DescriptionRequired;

        return new SiteLocalizedContent(language, name.Trim(), description.Trim());
    }

    internal Result Update(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return SiteErrors.NameRequired;
        
        if (string.IsNullOrWhiteSpace(description))
            return SiteErrors.DescriptionRequired;

        Name = name.Trim();
        Description = description.Trim();
        MarkAsUpdated();

        return Result.Success();
    }

}
