using Domain.Common.Interfaces;
using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.EventHandlers.Reviews;

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
                "Site {SiteId} not found while processing ReviewApprovedEvent",
                domainEvent);

            return;
        }

        site.AddRating(domainEvent.Rating);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
