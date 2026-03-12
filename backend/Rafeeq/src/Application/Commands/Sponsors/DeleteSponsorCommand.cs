using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;

namespace Application.Commands.Sponsors;

public record DeleteSponsorCommand(Guid Id) : ICommand;

internal class DeleteSponsorCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteSponsorCommand>
{
    public async Task<Result> HandleAsync(DeleteSponsorCommand command, CancellationToken cancellationToken)
    {
        var sponsor = await unitOfWork.Sponsors.GetByIdAsync(command.Id, cancellationToken);
        if (sponsor == null)
            return SponsorErrors.NotFound(command.Id);
        
        await unitOfWork.Sponsors.DeleteAsync(sponsor, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}