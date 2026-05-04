using Domain.Common.Interfaces;
using Domain.Entities.TouristAggregate;
using Microsoft.Extensions.Logging;

namespace Application.EventHandlers.Users;

internal class RatingUpdatedEventHandler(
    IUnitOfWork unitOfWork,
    ILogger<RatingUpdatedEventHandler> logger) : IDomainEventHandler<RatingUpdatedEvent>
{
    public async Task HandleAsync(RatingUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var site = await unitOfWork.Sites.GetByIdAsync(domainEvent.SiteId, cancellationToken);
        if (site == null)
        {
            logger.LogWarning(
                "Site with ID {SiteId} not found for RatingUpdatedEvent with VisitedSite ID {VisitedSiteId}",
                domainEvent.SiteId, domainEvent.VisitedSiteId);

            return;
        }

        site.UpdateRating(domainEvent.OldRating, domainEvent.NewRating);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
