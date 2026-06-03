using Domain.Common.Interfaces;
using Domain.Entities.ReviewAggregate;
using Microsoft.Extensions.Logging;

namespace Application.EventHandlers;

internal class ReviewApprovedEventHandler(
    IUnitOfWork unitOfWork,
    ILogger<ReviewApprovedEventHandler> logger) : IDomainEventHandler<ReviewApprovedEvent>
{
    public async Task HandleAsync(ReviewApprovedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var site = await unitOfWork.Sites.GetByIdAsync(domainEvent.SiteId, cancellationToken);
        if (site == null)
        {
            logger.LogWarning(
                "Site with ID {SiteId} not found for ReviewApprovedEvent with Review ID {ReviewId} and User ID {UserId}",
                domainEvent.SiteId, domainEvent.ReviewId, domainEvent.UserId);

            return;
        }

        site.AddRating(domainEvent.Rating);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

internal class ReviewUpdatedEventHandler(
    IUnitOfWork unitOfWork,
    ILogger<ReviewUpdatedEventHandler> logger) : IDomainEventHandler<ReviewUpdatedEvent>
{
    public async Task HandleAsync(ReviewUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var site = await unitOfWork.Sites.GetByIdAsync(domainEvent.SiteId, cancellationToken);
        if (site == null)
        {
            logger.LogWarning(
                "Site with ID {SiteId} not found for ReviewUpdatedEvent with Review ID {ReviewId}",
                domainEvent.SiteId, domainEvent.ReviewId);

            return;
        }

        site.UpdateRating(domainEvent.OldRating, domainEvent.NewRating);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}


internal class ReviewDeletedEventHandler(
    IUnitOfWork unitOfWork,
    ILogger<ReviewDeletedEventHandler> logger) : IDomainEventHandler<ReviewDeletedEvent>
{
    public async Task HandleAsync(ReviewDeletedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var site = await unitOfWork.Sites.GetByIdAsync(domainEvent.SiteId, cancellationToken);
        if (site == null)
        {
            logger.LogWarning(
                "Site with ID {SiteId} not found for ReviewDeletedEvent with Review ID {ReviewId}",
                domainEvent.SiteId, domainEvent.ReviewId);

            return;
        }

        site.RemoveRating(domainEvent.Rating);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}