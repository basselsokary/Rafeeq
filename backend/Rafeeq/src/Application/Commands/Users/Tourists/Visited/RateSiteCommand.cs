using Application.Common.Interfaces.Authentication;
using Domain.Common.Interfaces;
using Domain.Entities.TouristAggregate;
using Domain.ValueObjects;

namespace Application.Commands.Users.Tourists.Visited;

public sealed record RateSiteCommand(
    Guid SiteId,
    int Rating) : ICommand;

internal sealed class RateSiteCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext currentUserService) : ICommandHandler<RateSiteCommand>
{
    public async Task<Result> HandleAsync(RateSiteCommand command, CancellationToken cancellationToken)
    {
        var visitedSite = await unitOfWork.Tourists.GetVisitedSiteAsync(currentUserService.Id, command.SiteId, cancellationToken);
        if (visitedSite is null)
            return TouristErrors.VisitedSiteNotFound;

        var ratingResult = Rating.Create(command.Rating);
        if (ratingResult.Failed)
            return ratingResult;

        if (visitedSite.IsRated())
        {
            visitedSite.UpdateRating(ratingResult.Value);
        }
        else
        {
            visitedSite.SetRating(ratingResult.Value);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}