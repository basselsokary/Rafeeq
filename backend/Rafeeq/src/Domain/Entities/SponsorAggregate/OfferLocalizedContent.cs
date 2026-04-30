using Domain.Common;
using Domain.Enums;
using Shared;

namespace Domain.Entities.SponsorAggregate;

public class OfferLocalizedContent : BaseAuditableEntity
{
    public LanguageCode Language { get; private set; }
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string? TermsAndConditions { get; private set; }

    private OfferLocalizedContent() { }
    private OfferLocalizedContent(LanguageCode language, string title, string description, string? termsAndConditions)
    {
        Language = language;
        Title = title;
        Description = description;
        TermsAndConditions = termsAndConditions;
    }

    internal static Result<OfferLocalizedContent> Create(
        LanguageCode language,
        string title,
        string description,
        string? termsAndConditions)
    {    
        if (string.IsNullOrWhiteSpace(title))
            return SponsorErrors.TitleRequired;
        
        if (string.IsNullOrWhiteSpace(description))
            return SponsorErrors.DescriptionRequired;

        return new OfferLocalizedContent(language, title.Trim(), description.Trim(), termsAndConditions?.Trim());
    }

    internal Result<OfferLocalizedContent> Update(string title, string description, string? termsAndConditions)
    {
        if (string.IsNullOrWhiteSpace(title))
            return SponsorErrors.TitleRequired;
        
        if (string.IsNullOrWhiteSpace(description))
            return SponsorErrors.DescriptionRequired;

        Title = title.Trim();
        Description = description.Trim();
        TermsAndConditions = termsAndConditions?.Trim();

        return Result.Success(this);
    }
}
