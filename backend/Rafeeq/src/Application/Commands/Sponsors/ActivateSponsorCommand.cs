using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;

namespace Application.Commands.Sponsors;

public sealed record ActivateSponsorCommand(
    Guid Id,
    bool Active) : ICommand;

internal sealed class ActivateSponsorCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<ActivateSponsorCommand>
{
    public async Task<Result> HandleAsync(ActivateSponsorCommand command, CancellationToken cancellationToken)
    {
        var sponsor = await unitOfWork.Sponsors.GetByIdAsync(command.Id, cancellationToken);
        if (sponsor == null)
            return SponsorErrors.NotFound(command.Id);
        
        if (command.Active)
            sponsor.Activate();
        else 
            sponsor.Deactivate();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

