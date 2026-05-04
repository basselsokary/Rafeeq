using Domain.Common;
using Domain.Enums;
using Shared;

namespace Domain.Entities.ArtifactAggregate;

public class ArtifactLocalizedContent : BaseAuditableEntity
{
    public LanguageCode Language { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;

    private ArtifactLocalizedContent() { }
    private ArtifactLocalizedContent(LanguageCode language, string name, string description)
    {
        Language = language;
        Name = name;
        Description = description;
    }

    public static Result<ArtifactLocalizedContent> Create(LanguageCode language, string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ArtifactErrors.NameRequired;
        
        if (string.IsNullOrWhiteSpace(description))
            return ArtifactErrors.DescriptionRequired;

        return new ArtifactLocalizedContent(language, name.Trim(), description.Trim());
    }

    internal Result Update(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ArtifactErrors.NameRequired;
        
        if (string.IsNullOrWhiteSpace(description))
            return ArtifactErrors.DescriptionRequired;

        Name = name.Trim();
        Description = description.Trim();

        return Result.Success();
    }
}
