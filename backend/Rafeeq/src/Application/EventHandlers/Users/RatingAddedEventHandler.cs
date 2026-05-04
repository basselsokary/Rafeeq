using Domain.Common.Interfaces;
using Domain.Entities.TouristAggregate;
using Microsoft.Extensions.Logging;

namespace Application.EventHandlers.Users;

internal class RatingAddedEventHandler(
    IUnitOfWork unitOfWork,
    ILogger<RatingAddedEventHandler> logger) : IDomainEventHandler<RatingAddedEvent>
{
    public async Task HandleAsync(RatingAddedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var site = await unitOfWork.Sites.GetByIdAsync(domainEvent.SiteId, cancellationToken);
        if (site == null)
        {
            logger.LogWarning(
                "Site with ID {SiteId} not found for RatingAddedEvent with VisitedSiteId {VisitedSiteId} and User ID {UserId}",
                domainEvent.SiteId, domainEvent.VisitedSiteId, domainEvent.UserId);

            return;
        }

        site.AddRating(domainEvent.Rating);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
