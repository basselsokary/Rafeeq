using Microsoft.Extensions.DependencyInjection;

namespace Domain.Common.Interfaces;

internal sealed class DomainEventsDispatcher(IServiceProvider serviceProvider)
{
    public async Task DispatchAsync(
        IEnumerable<BaseEvent> domainEvents,
        CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        foreach (var domainEvent in domainEvents)
        {
            var handlerType = typeof(IDomainEventHandler<>)
                .MakeGenericType(domainEvent.GetType());

            var handlers = scopedProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                if (handler == null)
                    continue;

                await ((dynamic)handler).HandleAsync((dynamic)domainEvent, cancellationToken);
            }
        }
    }
}

