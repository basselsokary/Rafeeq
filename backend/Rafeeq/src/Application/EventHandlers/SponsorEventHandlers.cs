using Application.Common.Interfaces.Services;
using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;

namespace Application.EventHandlers;

internal class SponsorCreatedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<SponsorCreatedEvent>
{
    public async Task HandleAsync(SponsorCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.RemoveByPrefixAsync("sponsor:list"),
            cacheService.RemoveByPrefixAsync("sponsor:dashboard"));
    }
}

internal class SponsorUpdatedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<SponsorUpdatedEvent>
{
    public async Task HandleAsync(SponsorUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.RemoveByIdAsync("sponsor", domainEvent.SponsorId.ToString()),
            cacheService.RemoveByPrefixAsync("sponsor:list"),
            cacheService.RemoveByPrefixAsync("sponsor:dashboard"));
    }
}

internal class SponsorDeletedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<SponsorDeletedEvent>
{
    public async Task HandleAsync(SponsorDeletedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.RemoveByIdAsync("sponsor", domainEvent.SponsorId.ToString()),
            cacheService.RemoveByPrefixAsync("sponsor:list"),
            cacheService.RemoveByPrefixAsync("sponsor:dashboard"));
    }
}

internal class SponsorTierChangedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<SponsorTierChangedEvent>
{
    public async Task HandleAsync(SponsorTierChangedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.RemoveByIdAsync("sponsor", domainEvent.SponsorId.ToString()),
            cacheService.RemoveByPrefixAsync("sponsor:list"),
            cacheService.RemoveByPrefixAsync("sponsor:dashboard"));
    }
}

internal class SponsorImageUpdatedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<SponsorImageUpdatedEvent>
{
    public async Task HandleAsync(SponsorImageUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.RemoveByIdAsync("sponsor", domainEvent.SponsorId.ToString()),
            cacheService.RemoveByPrefixAsync("sponsor:list"),
            cacheService.RemoveByPrefixAsync("sponsor:dashboard"));
    }
}

internal class SponsorLocalizedContentUpdatedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<SponsorLocalizedContentUpdatedEvent>
{
    public async Task HandleAsync(SponsorLocalizedContentUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.RemoveByIdAsync("sponsor", domainEvent.SponsorId.ToString()),
            cacheService.RemoveByPrefixAsync("sponsor:list"),
            cacheService.RemoveByPrefixAsync("sponsor:dashboard"));
    }
}

internal class SponsorOfferChangedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<SponsorOfferChangedEvent>
{
    public async Task HandleAsync(SponsorOfferChangedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.RemoveByIdAsync("sponsor", domainEvent.SponsorId.ToString()),
            cacheService.RemoveByPrefixAsync("sponsor:offers"),
            cacheService.RemoveByPrefixAsync("sponsor:offer"),
            cacheService.RemoveByPrefixAsync("sponsor:list"),
            cacheService.RemoveByPrefixAsync("sponsor:dashboard"));
    }
}
