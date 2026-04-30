using Domain.Common.Interfaces;
using Domain.Entities.TouristAggregate;
using Microsoft.Extensions.Logging;

namespace Application.EventHandlers.Users;

internal class RatingRemovedEventHandler(
    IUnitOfWork unitOfWork,
    ILogger<RatingRemovedEventHandler> logger) : IDomainEventHandler<RatingRemovedEvent>
{
    public async Task HandleAsync(RatingRemovedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var site = await unitOfWork.Sites.GetByIdAsync(domainEvent.SiteId, cancellationToken);
        if (site == null)
        {
            logger.LogWarning(
                "Site with ID {SiteId} not found for RatingRemovedEvent with VisitedSite ID {VisitedSiteId}",
                domainEvent.SiteId, domainEvent.VisitedSiteId);

            return;
        }

        site.RemoveRating(domainEvent.Rating);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
