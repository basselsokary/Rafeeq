using Application.Common.Interfaces.Authentication;
using Domain.Common.Interfaces;
using Domain.Entities.TouristAggregate;

namespace Application.Commands.Users.Tourists.Visited;

public sealed record RemoveRatingCommand(
    Guid SiteId) : ICommand;

internal sealed class RemoveRatingCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext) : ICommandHandler<RemoveRatingCommand>
{

    public async Task<Result> HandleAsync(RemoveRatingCommand command, CancellationToken cancellationToken = default)
    {
        var visitedSite = await unitOfWork.Tourists.GetVisitedSiteAsync(userContext.Id, command.SiteId, cancellationToken);
        if (visitedSite == null)
            return TouristErrors.VisitedSiteNotFound;

        visitedSite.RemoveRating();
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}