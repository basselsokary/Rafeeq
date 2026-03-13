using Domain.Common.Interfaces;
using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.EventHandlers.Reviews;

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
                "Site {SiteId} not found while processing ReviewUpdatedEvent",
                domainEvent);

            return;
        }

        site.UpdateRating(domainEvent.OldRating, domainEvent.NewRating);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
