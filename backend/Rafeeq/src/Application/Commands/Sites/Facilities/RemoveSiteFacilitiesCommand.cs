using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;
using Domain.Enums;

namespace Application.Commands.Sites.Facilities;

public sealed record RemoveSiteFacilitiesCommand(
    Guid Id,
    List<FacilityType> FacilityTypes) : ICommand;

internal sealed class RemoveSiteFacilitiesCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveSiteFacilitiesCommand>
{
    public async Task<Result> HandleAsync(RemoveSiteFacilitiesCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetByIdAsync(command.Id, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.Id);

        site.RemoveFacilities(command.FacilityTypes);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}