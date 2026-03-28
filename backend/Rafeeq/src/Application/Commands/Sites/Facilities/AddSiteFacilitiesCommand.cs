using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;

namespace Application.Commands.Sites.Facilities;

public record AddSiteFacilitiesCommand(
    Guid Id,
    List<AddSiteFacilitiesDtoCommand> Facilities) : ICommand;

public record AddSiteFacilitiesDtoCommand(
    string FacilityName,
    string FacilityDescription) : ICommand;

internal class AddSiteFacilitiesCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<AddSiteFacilitiesCommand>
{
    public async Task<Result> HandleAsync(AddSiteFacilitiesCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetWithFacilitiesAsync(command.Id, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.Id);

        foreach (var facility in command.Facilities)
        {
            Result result = site.AddFacility(facility.FacilityName, facility.FacilityDescription);
            if (result.Failed)
                return result;
        }   

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}