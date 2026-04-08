using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;

namespace Application.Commands.Sites.Facilities;

public record RemoveSiteFacilitiesCommand(
    Guid Id,
    List<Guid> FacilityIds) : ICommand;

internal class RemoveSiteFacilitiesCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveSiteFacilitiesCommand>
{
    public async Task<Result> HandleAsync(RemoveSiteFacilitiesCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetWithFacilitiesAsync(command.Id, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.Id);

        foreach (var facilityId in command.FacilityIds)
        {
            Result result = site.RemoveFacility(facilityId);
            if (result.Failed)
                return result;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}