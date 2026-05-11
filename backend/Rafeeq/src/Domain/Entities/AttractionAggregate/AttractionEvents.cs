using Domain.Common;

namespace Domain.Entities.AttractionAggregate;

public class AttractionCreatedEvent(Guid attractionId) : BaseEvent
{
    public Guid AttractionId { get; } = attractionId;
}

public class AttractionUpdatedEvent(Guid attractionId) : BaseEvent
{
    public Guid AttractionId { get; } = attractionId;
}

public class AttractionDeletedEvent(Guid attractionId) : BaseEvent
{
    public Guid AttractionId { get; } = attractionId;
}

public class AttractionImageUpdatedEvent(Guid attractionId) : BaseEvent
{
    public Guid AttractionId { get; } = attractionId;
}

public class AttractionLocalizedContentUpdatedEvent(Guid attractionId) : BaseEvent
{
    public Guid AttractionId { get; } = attractionId;
}

// public class AttractionStatusChangedEvent(Guid AttractionId, AttractionStatus NewStatus) : BaseEvent
// {
//     public Guid AttractionId { get; } = AttractionId;
//     public AttractionStatus NewStatus { get; } = NewStatus;
// }