using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;
using Domain.Enums;

namespace Application.Commands.Sites.Facilities;

public sealed record AddSiteFacilitiesCommand(
    Guid Id,
    List<FacilityType> FacilityTypes) : ICommand;

internal sealed class AddSiteFacilitiesCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<AddSiteFacilitiesCommand>
{
    public async Task<Result> HandleAsync(AddSiteFacilitiesCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetByIdAsync(command.Id, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.Id);
        
        var result = site.AddFacilities(command.FacilityTypes);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}