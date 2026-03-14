using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;

namespace Application.Commands.Sites.Facilities;

public record RemoveSiteFacilityCommand(
    Guid Id,
    Guid FacilityId) : ICommand;

internal class RemoveSiteFacilityCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveSiteFacilityCommand>
{
    public async Task<Result> HandleAsync(RemoveSiteFacilityCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetWithFacilitiesAsync(command.Id, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.Id);

        Result result = site.RemoveFacility(command.FacilityId);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}