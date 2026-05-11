using Domain.Common.Interfaces;
using Domain.Entities.ReviewAggregate;
using Domain.Entities.TouristAggregate;
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
                "Site with ID {SiteId} not found for ReviewUpdatedEvent with Review ID {ReviewId}",
                domainEvent.SiteId, domainEvent.ReviewId);

            return;
        }

        site.UpdateRating(domainEvent.OldRating, domainEvent.NewRating);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
