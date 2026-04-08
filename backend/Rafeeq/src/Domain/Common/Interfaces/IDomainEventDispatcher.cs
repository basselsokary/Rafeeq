namespace Domain.Common.Interfaces;

/// <summary>
/// Interface for dispatching domain events
/// </summary>
public interface IDomainEventDispatcher
{
    Task DispatchAsync(BaseEvent domainEvent, CancellationToken cancellationToken = default);
    Task DispatchAsync(IEnumerable<BaseEvent> domainEvents, CancellationToken cancellationToken = default);
}
