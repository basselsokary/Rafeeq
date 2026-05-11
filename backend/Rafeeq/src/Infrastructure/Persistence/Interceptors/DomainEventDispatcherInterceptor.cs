using Domain.Common;
using Infrastructure.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Interceptors;

internal sealed class DomainEventDispatcherInterceptor(
    DomainEventsDispatcher dispatcher,
    ILogger<DomainEventDispatcherInterceptor> logger) : SaveChangesInterceptor
{
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context != null)
        {
            await DispatchDomainEventsAsync(eventData.Context, cancellationToken);
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(
        SaveChangesCompletedEventData eventData,
        int result)
    {
        if (eventData.Context != null)
        {
            DispatchDomainEventsAsync(eventData.Context, CancellationToken.None).GetAwaiter().GetResult();
        }

        return base.SavedChanges(eventData, result);
    }

    private async Task DispatchDomainEventsAsync(
        DbContext context,
        CancellationToken cancellationToken)
    {
        var domainEntities = context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(x => x.Entity.DomainEvents.Count > 0)
            .Select(x => x.Entity)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.DomainEvents)
            .ToList();

        logger.LogInformation(
            "Dispatching {Count} domain events from {EntityCount} entities.",
            domainEvents.Count, domainEntities.Count);

        domainEntities.ForEach(entity =>
        {
            logger.LogInformation(
                "Clearing {EventCount} domain events from entity {EntityType}.",
                entity.DomainEvents.Count, entity.GetType().Name);
            entity.ClearDomainEvents();
        });

        await dispatcher.DispatchAsync(domainEvents, cancellationToken);
    }
}
