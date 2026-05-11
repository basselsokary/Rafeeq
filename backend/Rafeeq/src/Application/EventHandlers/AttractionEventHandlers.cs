using Application.Common.Interfaces.Services;
using Domain.Common.Interfaces;
using Domain.Entities.AttractionAggregate;

namespace Application.EventHandlers;

internal class AttractionCreatedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<AttractionCreatedEvent>
{
    public async Task HandleAsync(AttractionCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.RemoveByPrefixAsync("attraction:list"));
    }
}

internal class AttractionUpdatedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<AttractionUpdatedEvent>
{
    public async Task HandleAsync(AttractionUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.RemoveByIdAsync("attraction", domainEvent.AttractionId.ToString()),
            cacheService.RemoveByPrefixAsync("attraction:list"));
    }
}

internal class AttractionDeletedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<AttractionDeletedEvent>
{
    public async Task HandleAsync(AttractionDeletedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.RemoveByIdAsync("attraction", domainEvent.AttractionId.ToString()),
            cacheService.RemoveByPrefixAsync("attraction:list"));
    }
}

internal class AttractionImageUpdatedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<AttractionImageUpdatedEvent>
{
    public async Task HandleAsync(AttractionImageUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.RemoveByIdAsync("attraction", domainEvent.AttractionId.ToString()),
            cacheService.RemoveByPrefixAsync("attraction:list"));
    }
}

internal class AttractionLocalizedContentUpdatedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<AttractionLocalizedContentUpdatedEvent>
{
    public async Task HandleAsync(AttractionLocalizedContentUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.RemoveByIdAsync("attraction", domainEvent.AttractionId.ToString()),
            cacheService.RemoveByPrefixAsync("attraction:list"));
    }
}
