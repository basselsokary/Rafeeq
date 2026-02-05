using Domain.Common;
using Domain.ValueObjects;
using EventMaster.Domain.Enums;
using EventMaster.Domain.Errors;
using EventMaster.Domain.Events;
using Shared.Models;

namespace EventMaster.Domain.Entities;

public class Event : BaseAuditableEntity, IAggregateRoot
{
    private Event() { }
    private Event(
        string organizerId,
        string title, string description, string venue, string location,
        Money ticketPrice,
        int totalTickets,
        DateTime date,
        List<EventAttachment> eventAttachments)
            : this(organizerId, title, description, venue, location, ticketPrice, totalTickets, date)
    {
        _eventAttachments = eventAttachments;
    }
    private Event(
        string organizerId,
        string title, string description, string venue, string location,
        Money ticketPrice,
        int totalTickets,
        DateTime date)
    {
        OrganizerId = organizerId;

        Title = title;
        Description = description;
        Venue = venue;
        Location = location;

        TicketPrice = ticketPrice;

        TotalTickets = totalTickets;
        TicketsLeft = totalTickets;

        Date = date;

        Status = EventStatus.Pending;
    }

    public string OrganizerId { get; private set; } = null!;

    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string Venue { get; private set; } = null!;
    public string Location { get; private set; } = null!;

    public Money TicketPrice { get; private set; } = null!;

    public int TotalTickets { get; private set; }
    public int TicketsLeft { get; private set; }

    public EventStatus Status { get; private set; }

    public DateTime Date { get; private set; }

    private readonly List<EventAttachment> _eventAttachments = [];
    public IReadOnlyCollection<EventAttachment> EventAttachments => _eventAttachments.AsReadOnly();

    public static Event Create(
       string organizerId,
       string title, string description, string venue, string location,
       Money ticketPrice,
       int totalTickets,
       DateTime date,
       List<EventAttachment>? eventAttachments = null)
    {
        if (string.IsNullOrWhiteSpace(organizerId))
            throw new ArgumentException("Organizer ID cannot be null or empty.", nameof(organizerId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be null or empty.", nameof(title));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be null or empty.", nameof(description));
        if (string.IsNullOrWhiteSpace(venue))
            throw new ArgumentException("Venue cannot be null or empty.", nameof(venue));
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be null or empty.", nameof(location));

        if (totalTickets <= 0)
            throw new ArgumentOutOfRangeException(nameof(totalTickets), "Total tickets cannot be less than or equal 0.");

        if (date == default || date < DateTime.UtcNow)
            throw new ArgumentException("Date must be set to valid value.", nameof(date));

        if (eventAttachments == null)
            return new(
                organizerId,
                title, description, venue, location,
                ticketPrice,
                totalTickets,
                date);

        return new(
            organizerId,
            title, description, venue, location,
            ticketPrice,
            totalTickets,
            date,
            eventAttachments);
    }

    public void Update(
        string title, string description, string venue, string location,
        Money money,
        int totalTickets,
        DateTime date)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be null or empty.", nameof(title));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be null or empty.", nameof(description));
        if (string.IsNullOrWhiteSpace(venue))
            throw new ArgumentException("Venue cannot be null or empty.", nameof(venue));
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be null or empty.", nameof(location));

        if (totalTickets < TotalTickets)
            throw new ArgumentOutOfRangeException(nameof(totalTickets), "Total tickets cannot be less to current tickets left.");

        if (date == default || date < DateTime.UtcNow)
            throw new ArgumentException("Date must be set to valid value.", nameof(date));

        Title = title;
        Description = description;
        Venue = venue;
        Location = location;

        TicketPrice = money;

        TotalTickets = totalTickets;
        TicketsLeft = totalTickets;

        Date = date;

        RaiseDomainEvent(new EventUpdatedEvent(this));
    }

    public Result<EventAttachment> AddAttachment(EventAttachment attachment)
    {
        if (_eventAttachments.Any(ea => ea.FileUrl == attachment.FileUrl))
            return Result.Failure<EventAttachment>(EventErrors.DuplicateAttachment(attachment.FileUrl));

        _eventAttachments.Add(attachment);
        RaiseDomainEvent(new EventAttachmentAddedEvent(this, attachment));

        return Result.Success(attachment);
    }

    public Result DeleteAttachment(Guid id)
    {
        var attachment = _eventAttachments.FirstOrDefault(ea => ea.Id == id);
        if (attachment != null)
        {
            _eventAttachments.Remove(attachment);
            return Result.Success();
        }

        return Result.Failure(["Attachment not found."]);
    }

    public Result DeleteAttachment(EventAttachment attachment)
    {
        return DeleteAttachment(attachment.Id);
    }

    public Result Approve(bool isApproved = true)
    {
        if (Status == EventStatus.Approved)
            return Result.Failure(EventErrors.AlreadyApproved(Id));

        if (isApproved)
            Status = EventStatus.Approved;
        else
            Status = EventStatus.Rejected;

        // RaiseDomainEvent(new EventApprovedEvent(this));

        return Result.Success();
    }

    public Result PurshaseTicket(Ticket ticket)
    {
        if (TicketsLeft < 1)
            return Result.Failure(TicketErrors.NoTicketsLeft());

        TicketsLeft -= 1;
        // RaiseDomainEvent(new TicketPurchasedEvent(this, money));

        return Result.Success();
    }
}
