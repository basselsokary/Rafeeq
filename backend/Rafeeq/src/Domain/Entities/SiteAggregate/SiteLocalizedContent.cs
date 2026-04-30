using Domain.Common;
using Domain.Common.Constants;
using Domain.Enums;
using Domain.ValueObjects;
using Shared;

namespace Domain.Entities.SiteAggregate;

public class SiteLocalizedContent : BaseAuditableEntity
{
    public LanguageCode Language { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public Address Address { get; private set; } = null!;
    public string? EntryTicketNotes { get; private set; }

    private SiteLocalizedContent() { }
    private SiteLocalizedContent(
        LanguageCode language,
        string name,
        string description,
        Address address,
        string? entryTicketNotes)
    {
        Language = language;
        Name = name;
        Description = description;
        EntryTicketNotes = entryTicketNotes;
        
        Address = address;
    }

    internal static Result<SiteLocalizedContent> Create(
        LanguageCode language,
        string name,
        string description,
        Address address,
        string? entryTicketNotes)
    {    
        if (string.IsNullOrWhiteSpace(name))
            return SiteErrors.NameRequired;
        
        if (string.IsNullOrWhiteSpace(description))
            return SiteErrors.DescriptionRequired;
        
        if (!string.IsNullOrWhiteSpace(entryTicketNotes) && entryTicketNotes.Length > DomainConstants.Ticket.MaxNotesLength)
            return TicketErrors.ExceededNotesLength;

        return new SiteLocalizedContent(language, name.Trim(), description.Trim(), address, entryTicketNotes?.Trim());
    }

    internal Result Update(string name, string description, string? entryTicketNotes)
    {
        if (string.IsNullOrWhiteSpace(name))
            return SiteErrors.NameRequired;
        
        if (string.IsNullOrWhiteSpace(description))
            return SiteErrors.DescriptionRequired;
        
        if (!string.IsNullOrWhiteSpace(entryTicketNotes) && entryTicketNotes.Length > DomainConstants.Ticket.MaxNotesLength)
            return TicketErrors.ExceededNotesLength;

        Name = name.Trim();
        Description = description.Trim();
        EntryTicketNotes = entryTicketNotes?.Trim();

        return Result.Success();
    }

    internal void UpdateAddress(Address address)
    {
        Address = address;
    }
}
