using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Commands.Sponsors;

public record UpdateSponsorCommand(
    Guid Id,
    string Title,
    string Description,
    SponsorType Type,
    SponsorTier Tier,
    GeoLocation Location,
    Address Address,
    DateTime StartDate,
    DateTime EndDate
) : ICommand;

internal class UpdateSponsorCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateSponsorCommand>
{
    public async Task<Result> HandleAsync(UpdateSponsorCommand command, CancellationToken cancellationToken)
    {
        var sponsor = await unitOfWork.Sponsors.GetByIdAsync(command.Id, cancellationToken);
        if (sponsor == null)
            return SponsorErrors.NotFound(command.Id);
        
        Result result = ApplyChanges(command, sponsor);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static Result ApplyChanges(UpdateSponsorCommand command, Sponsor sponsor)
    {
        sponsor.UpdateTier(command.Tier);
        sponsor.UpdateLocation(command.Location, command.Address);
        Result result = sponsor.UpdateBasicInfo(command.Title, command.Description, command.Type);
        if (result.Failed)
            return result;

        return Result.Success();
    }
}