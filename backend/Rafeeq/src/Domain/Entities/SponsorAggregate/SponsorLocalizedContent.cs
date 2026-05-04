using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;
using Shared;

namespace Domain.Entities.SponsorAggregate;

public class SponsorLocalizedContent : BaseAuditableEntity
{
    public LanguageCode Language { get; private set; }
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public Address Address { get; private set; } = null!;

    private SponsorLocalizedContent() { }
    private SponsorLocalizedContent(LanguageCode language, string title, string description, Address address)
    {
        Language = language;
        Title = title;
        Description = description;

        Address = address;
    }

    internal static Result<SponsorLocalizedContent> Create(
        LanguageCode language,
        string title,
        string description,
        Address address)
    {    
        if (string.IsNullOrWhiteSpace(title))
            return SponsorErrors.TitleRequired;
        
        if (string.IsNullOrWhiteSpace(description))
            return SponsorErrors.DescriptionRequired;

        return new SponsorLocalizedContent(language, title.Trim(), description.Trim(), address);
    }

    internal Result Update(string title, string description)
    {
        if (string.IsNullOrWhiteSpace(title))
            return SponsorErrors.TitleRequired;
        
        if (string.IsNullOrWhiteSpace(description))
            return SponsorErrors.DescriptionRequired;

        Title = title.Trim();
        Description = description.Trim();

        return Result.Success();
    }

    internal void UpdateAddress(Address address)
    {
        if (address != Address)
        {
            Address = address;
        }
    }
}
