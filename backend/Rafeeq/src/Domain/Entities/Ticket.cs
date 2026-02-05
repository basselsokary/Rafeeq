using Domain.Common;
using Domain.ValueObjects;
using EventMaster.Domain.Enums;

namespace EventMaster.Domain.Entities;

public class Ticket : BaseAuditableEntity
{
    private Ticket() { }
    private Ticket(string participantId, Guid eventId, Money price)
    {
        ParticipantId = participantId;
        EventId = eventId;

        Price = price;
        
        Status = TicketStatus.Active;
    }

    public string ParticipantId { get; private set; } = string.Empty;

    public Guid EventId { get; private set; }

    public Money Price { get; private set; } = null!;

    public TicketStatus Status { get; private set; }

    public static Ticket Create(string participantId, Guid eventId, Money price)
    {
        if (string.IsNullOrWhiteSpace(participantId))
            throw new ArgumentException("Participant ID cannot be null or empty.", nameof(participantId));

        if (eventId == default)
            throw new ArgumentException("Event ID cannot be empty.", nameof(eventId));

        return new(participantId, eventId, price);   
    }
}
