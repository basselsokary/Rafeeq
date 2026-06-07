using Application.Common.Interfaces.Services;
using Domain.Common.Interfaces;
using Domain.Entities.ArtifactAggregate;

namespace Application.EventHandlers;

internal class ArtifactCreatedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<ArtifactCreatedEvent>
{
    public async Task HandleAsync(ArtifactCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.RemoveByPrefixAsync("artifact:list"));
    }
}

internal class ArtifactUpdatedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<ArtifactUpdatedEvent>
{
    public async Task HandleAsync(ArtifactUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.RemoveByIdAsync("artifact", domainEvent.ArtifactId.ToString()),
            cacheService.RemoveByPrefixAsync("artifact:list"));
    }
}

internal class ArtifactDeletedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<ArtifactDeletedEvent>
{
    public async Task HandleAsync(ArtifactDeletedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.RemoveByIdAsync("artifact", domainEvent.ArtifactId.ToString()),
            cacheService.RemoveByPrefixAsync("artifact:list"));
    }
}

internal class ArtifactImageUpdatedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<ArtifactImageUpdatedEvent>
{
    public async Task HandleAsync(ArtifactImageUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.RemoveByIdAsync("artifact", domainEvent.ArtifactId.ToString()),
            cacheService.RemoveByPrefixAsync("artifact:list"));
    }
}

internal class ArtifactLocalizedContentUpdatedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<ArtifactLocalizedContentUpdatedEvent>
{
    public async Task HandleAsync(ArtifactLocalizedContentUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            cacheService.RemoveByIdAsync("artifact", domainEvent.ArtifactId.ToString()),
            cacheService.RemoveByPrefixAsync("artifact:list"));
    }
}
