using Application.Common.Interfaces.Services;
using Domain.Common.Interfaces;
using Domain.Entities.CityAggregate;

namespace Application.EventHandlers;

internal class CityCreatedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<CityCreatedEvent>
{
    public async Task HandleAsync(CityCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.RemoveByPrefixAsync("city:list"));
    }
}

internal class CityUpdatedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<CityUpdatedEvent>
{
    public async Task HandleAsync(CityUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.RemoveByIdAsync("city", domainEvent.CityId.ToString()),
            cacheService.RemoveByPrefixAsync("city:list"));
    }
}

internal class CityDeletedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<CityDeletedEvent>
{
    public async Task HandleAsync(CityDeletedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.RemoveByIdAsync("city", domainEvent.CityId.ToString()),
            cacheService.RemoveByPrefixAsync("city:list"));
    }
}

internal class CityLocalizedContentUpdatedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<CityLocalizedContentUpdatedEvent>
{
    public async Task HandleAsync(CityLocalizedContentUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.RemoveByIdAsync("city", domainEvent.CityId.ToString()),
            cacheService.RemoveByPrefixAsync("city:list"));
    }
}