using Domain.Events;

namespace Domain.Common.Interfaces;

/// <summary>
/// Interface for handling domain events
/// </summary>
public interface IDomainEventHandler<in TEvent>
    where TEvent : BaseEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}
