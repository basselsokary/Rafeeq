using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;

namespace Application.Commands.Sites.NearestTransportations;

public sealed record RemoveSiteNearestTransportationCommand(
    Guid SiteId,
    Guid TransportationId) : ICommand;

internal sealed class RemoveSiteNearestTransportationCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveSiteNearestTransportationCommand>
{
    public async Task<Result> HandleAsync(RemoveSiteNearestTransportationCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetWithNearestTransportationAsync(command.SiteId, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.SiteId);

        var result = site.RemoveNearestTransportation(command.TransportationId);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}