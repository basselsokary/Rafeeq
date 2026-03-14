using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;

namespace Application.Commands.Sponsors.Offers;

public record SetSponsorOfferPromoCodeCommand(
    Guid SponsorId,
    Guid OfferId,
    string PromoCode) : ICommand;

internal class SetSponsorOfferPromoCodeCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<SetSponsorOfferPromoCodeCommand>
{
    public async Task<Result> HandleAsync(SetSponsorOfferPromoCodeCommand command, CancellationToken cancellationToken)
    {

        var sponsor = await unitOfWork.Sponsors.GetWithOffers(command.SponsorId, cancellationToken);
        if (sponsor == null)
            return SponsorErrors.NotFound(command.SponsorId);

        Offer? offer = sponsor.Offers.FirstOrDefault(o => o.Id == command.OfferId);
        if (offer == null)
            return SponsorErrors.OfferNotFound(command.OfferId);
        
        Result result = offer.SetPromoCode(command.PromoCode);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}