using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;

namespace Application.Commands.Sponsors;

public record ExtendSponsorContractCommand(
    Guid Id,
    DateTime NewEndDate) : ICommand;

internal class ExtendSponsorContractCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<ExtendSponsorContractCommand>
{
    public async Task<Result> HandleAsync(ExtendSponsorContractCommand command, CancellationToken cancellationToken)
    {
        var sponsor = await unitOfWork.Sponsors.GetByIdAsync(command.Id, cancellationToken);
        if (sponsor == null)
            return SponsorErrors.NotFound(command.Id);
        
        Result result = sponsor.ExtendContract(command.NewEndDate);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}