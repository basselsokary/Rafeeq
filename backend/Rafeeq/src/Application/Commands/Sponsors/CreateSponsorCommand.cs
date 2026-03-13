using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Commands.Sponsors;

public record CreateSponsorCommand(
    string Title,
    string Description,
    SponsorType Type,
    SponsorTier Tier,
    GeoLocation Location,
    Address Address,
    DateTime StartDate,
    DateTime EndDate) : ICommand;

internal class CreateSponsorCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<CreateSponsorCommand>
{
    public async Task<Result> HandleAsync(CreateSponsorCommand command, CancellationToken cancellationToken)
    {
        Result<Sponsor> result = Sponsor.Create(
            command.Title,
            command.Description,
            command.Type,
            command.Tier,
            command.Location,
            command.Address,
            command.StartDate,
            command.EndDate);
        
        if (result.Failed)
            return result;

        await unitOfWork.Sponsors.AddAsync(result.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}