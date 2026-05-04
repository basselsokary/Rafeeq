using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;

namespace Application.Commands.Sponsors.Offers;

public sealed record RemoveSponsorOfferCommand(
    Guid SponsorId,
    Guid OfferId) : ICommand;

internal sealed class RemoveSponsorOfferCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveSponsorOfferCommand>
{
    public async Task<Result> HandleAsync(RemoveSponsorOfferCommand command, CancellationToken cancellationToken)
    {

        var sponsor = await unitOfWork.Sponsors.GetWithOffersAsync(command.SponsorId, cancellationToken);
        if (sponsor == null)
            return SponsorErrors.NotFound(command.SponsorId);

        Result result = sponsor.RemoveOffer(command.OfferId);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}