using Domain.Events;

namespace Domain.Common;

public abstract class BaseEvent : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
