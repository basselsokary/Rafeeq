using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;

namespace Application.Commands.Sponsors.Offers;

public record RedeemSponsorOfferCommand(
    Guid SponsorId,
    Guid OfferId) : ICommand;

internal class RedeemSponsorOfferCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<RedeemSponsorOfferCommand>
{
    public async Task<Result> HandleAsync(RedeemSponsorOfferCommand command, CancellationToken cancellationToken)
    {

        var sponsor = await unitOfWork.Sponsors.GetWithOffers(command.SponsorId, cancellationToken);
        if (sponsor == null)
            return SponsorErrors.NotFound(command.SponsorId);

        Result result = sponsor.RedeemOffer(command.OfferId);
        
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}