using Domain.Common.Interfaces;
using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.EventHandlers.Reviews;

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
                "Site {SiteId} not found while processing ReviewDeletedEvent",
                domainEvent);

            return;
        }

        site.RemoveRating(domainEvent.Rating);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
