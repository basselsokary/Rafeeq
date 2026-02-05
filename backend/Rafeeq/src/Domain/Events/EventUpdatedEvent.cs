using Domain.Common;
using EventMaster.Domain.Entities;

namespace EventMaster.Domain.Events;

public class EventUpdatedEvent : BaseEvent
{
    public EventUpdatedEvent(Event @event)
    {
        Event = @event ?? throw new ArgumentNullException(nameof(@event));
    }

    public Event Event { get; }
}
