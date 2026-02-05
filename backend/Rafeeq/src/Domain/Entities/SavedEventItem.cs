namespace EventMaster.Domain.Entities;

public class SavedEventItem
{
    private SavedEventItem() { }
    private SavedEventItem(Guid eventId)
    {
        EventId = eventId;
    }
    private SavedEventItem(Guid eventId, Guid basketId)
    {
        BasketId = basketId;
        EventId = eventId;
    }

    public Guid BasketId { get; private set; }

    public Guid EventId { get; private set; }

    public static SavedEventItem Create(Guid eventId, Guid basketId = default)
    {
        if (eventId == default)
            throw new ArgumentException("Event ID cannot be empty.", nameof(eventId));

        if (basketId == default)
            return new(eventId);

        return new(basketId, eventId);
    }
}
