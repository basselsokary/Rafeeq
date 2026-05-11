using Application.Common.Interfaces.Services;
using Domain.Common.Interfaces;
using Domain.Entities.TripAggregate;

namespace Application.EventHandlers;

internal class TripCreatedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<TripCreatedEvent>
{
    public async Task HandleAsync(TripCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.RemoveByPrefixAsync($"trip:touristId:{domainEvent.TouristId}"));
    }
}

internal class TripUpdatedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<TripUpdatedEvent>
{
    public async Task HandleAsync(TripUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.RemoveByPrefixAsync($"trip:touristId:{domainEvent.TouristId}"));
    }
}

internal class TripDeletedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<TripDeletedEvent>
{
    public async Task HandleAsync(TripDeletedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.RemoveByPrefixAsync($"trip:touristId:{domainEvent.TouristId}"));
    }
}