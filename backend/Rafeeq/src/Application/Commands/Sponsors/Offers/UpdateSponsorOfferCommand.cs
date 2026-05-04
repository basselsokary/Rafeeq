using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;
using Domain.ValueObjects;

namespace Application.Commands.Sponsors.Offers;

public sealed record UpdateSponsorOfferCommand(
    Guid SponsorId,
    Guid OfferId,
    decimal? DiscountAmount,
    int? DiscountPercentage,
    DateTime StartDate,
    DateTime EndDate,
    int? MaxRedemptions,
    string? PromoCode) : ICommand;

internal sealed class UpdateSponsorOfferCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateSponsorOfferCommand>
{
    public async Task<Result> HandleAsync(UpdateSponsorOfferCommand command, CancellationToken cancellationToken)
    {
        Result<Money>? moneyResult = null;
		if (command.DiscountAmount.HasValue)
		{
			moneyResult = Money.Create(command.DiscountAmount.Value);
			if (moneyResult.Failed)
				return moneyResult;
		}

		var dateRangeResult = DateRange.Create(command.StartDate, command.EndDate);
		if (dateRangeResult.Failed)
			return dateRangeResult;
        
        var sponsor = await unitOfWork.Sponsors.GetWithOffersAsync(command.SponsorId, cancellationToken);
        if (sponsor == null)
            return SponsorErrors.NotFound(command.SponsorId);
        
        var offer = sponsor.Offers.FirstOrDefault(o => o.Id == command.OfferId);
        if (offer == null)
            return SponsorErrors.OfferNotFound(command.OfferId);

        if (command.MaxRedemptions.HasValue)
        {
            offer.SetMaxRedemptions(command.MaxRedemptions.Value);
        }

        if (!string.IsNullOrEmpty(command.PromoCode))
        {
            var codeResult = offer.SetPromoCode(command.PromoCode);
            if (codeResult.Failed)
                return codeResult;
        }

        Result result = sponsor.UpdateOffer(
            command.OfferId,
            moneyResult?.Value,
            command.DiscountPercentage,
            dateRangeResult.Value);
        
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}