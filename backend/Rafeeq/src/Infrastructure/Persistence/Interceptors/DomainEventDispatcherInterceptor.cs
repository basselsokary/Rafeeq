using Domain.Common;
using Infrastructure.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Persistence.Interceptors;

internal sealed class DomainEventDispatcherInterceptor(
    DomainEventsDispatcher dispatcher) : SaveChangesInterceptor
{
    private List<BaseEvent> _domainEvents = [];

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            _domainEvents = GetDomainEvents(eventData.Context);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        await dispatcher.DispatchAsync(_domainEvents, cancellationToken);
        _domainEvents = [];

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private static List<BaseEvent> GetDomainEvents(DbContext context)
    {
        var domainEntities = context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(x => x.Entity.DomainEvents.Count > 0)
            .Select(x => x.Entity)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.DomainEvents)
            .ToList();

        domainEntities.ForEach(entity =>
        {
            entity.ClearDomainEvents();
        });

        return domainEvents;
    }

}
