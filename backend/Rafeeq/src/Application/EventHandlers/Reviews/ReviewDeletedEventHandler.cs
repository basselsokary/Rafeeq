using Domain.Common.Interfaces;
using Domain.Entities.ReviewAggregate;
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
                "Site with ID {SiteId} not found for ReviewDeletedEvent with Review ID {ReviewId}",
                domainEvent.SiteId, domainEvent.ReviewId);

            return;
        }

        site.RemoveRating(domainEvent.Rating);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
