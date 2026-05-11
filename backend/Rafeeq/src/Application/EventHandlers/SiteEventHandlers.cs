using Application.Common.Interfaces.Services;
using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;

namespace Application.EventHandlers;

internal class SiteCreatedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<SiteCreatedEvent>
{
    public async Task HandleAsync(SiteCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.RemoveByPrefixAsync("site:list"));
    }
}

internal class SiteUpdatedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<SiteUpdatedEvent>
{
    public async Task HandleAsync(SiteUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.RemoveByIdAsync("site", domainEvent.SiteId.ToString()),
            cacheService.RemoveByPrefixAsync("site:list"));
    }
}

internal class SiteDeletedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<SiteDeletedEvent>
{
    public async Task HandleAsync(SiteDeletedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.RemoveByIdAsync("site", domainEvent.SiteId.ToString()),
            cacheService.RemoveByPrefixAsync("site:list"));
    }
}

internal class SiteImageUpdatedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<SiteImageUpdatedEvent>
{
    public async Task HandleAsync(SiteImageUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.RemoveByIdAsync("site", domainEvent.SiteId.ToString()),
            cacheService.RemoveByPrefixAsync("site:list"));
    }
}

internal class SiteLocalizedContentUpdatedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<SiteLocalizedContentUpdatedEvent>
{
    public async Task HandleAsync(SiteLocalizedContentUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.RemoveByIdAsync("site", domainEvent.SiteId.ToString()),
            cacheService.RemoveByPrefixAsync("site:list"));
    }
}
