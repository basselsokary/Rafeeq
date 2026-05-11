using Domain.Common;

namespace Domain.Entities.TripAggregate;

public class TripCreatedEvent(Guid TripId, Guid TouristId) : BaseEvent
{
    public Guid TripId { get; } = TripId;
    public Guid TouristId { get; } = TouristId;
}

public class TripUpdatedEvent(Guid TripId, Guid TouristId) : BaseEvent
{
    public Guid TripId { get; } = TripId;
    public Guid TouristId { get; } = TouristId;
}

public class TripDeletedEvent(Guid TripId, Guid TouristId) : BaseEvent
{
    public Guid TripId { get; } = TripId;
    public Guid TouristId { get; } = TouristId;
}

// public class TripStatusChangedEvent(Guid TripId, TripStatus NewStatus) : BaseEvent
// {
//     public Guid EventId { get; } = Guid.NewGuid();
// }