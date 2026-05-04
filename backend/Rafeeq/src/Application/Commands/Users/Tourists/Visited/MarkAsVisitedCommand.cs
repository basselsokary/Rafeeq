using Application.Common.Interfaces.Authentication;
using Domain.Common.Interfaces;
using Domain.Entities.TouristAggregate;

namespace Application.Commands.Users.Tourists.Visited;

public sealed record MarkAsVisitedCommand(
    Guid SiteId,
    int DurationMinutes,
    DateTime VisitedAt) : ICommand;

internal sealed class MarkAsVisitedCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext currentUserService) : ICommandHandler<MarkAsVisitedCommand>
{
    public async Task<Result> HandleAsync(MarkAsVisitedCommand command, CancellationToken cancellationToken)
    {
        var tourist = await unitOfWork.Tourists.GetWithVisitedSitesAsync(currentUserService.Id, cancellationToken);
        if (tourist is null)
            return TouristErrors.NotFound(currentUserService.Id);

        var result = tourist.MarkSiteAsVisited(command.SiteId, command.DurationMinutes, command.VisitedAt);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}