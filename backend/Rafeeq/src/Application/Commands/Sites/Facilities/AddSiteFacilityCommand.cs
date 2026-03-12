using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;

namespace Application.Commands.Sites.Facilities;

public record AddSiteFacilityCommand(
    Guid Id,
    string FacilityName,
    string FacilityDescription) : ICommand;

internal class AddSiteFacilityCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<AddSiteFacilityCommand>
{
    public async Task<Result> HandleAsync(AddSiteFacilityCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetByIdAsync(command.Id, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.Id);

        Result result = site.AddFacility(command.FacilityName, command.FacilityDescription);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}