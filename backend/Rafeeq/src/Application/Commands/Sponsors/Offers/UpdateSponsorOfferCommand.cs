using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;
using Domain.ValueObjects;

namespace Application.Commands.Sponsors.Offers;

public record UpdateSponsorOfferCommand(
    Guid SponsorId,
    Guid OfferId,
    string Title,
    string Description,
    decimal? DiscountAmount,
    int? DiscountPercentage,
    DateTime StartDate,
    DateTime EndDate,
    string? TermsAndConditions,
    int? MaxRedemptions) : ICommand;

internal class UpdateSponsorOfferCommandHandler(
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

        if (command.MaxRedemptions.HasValue)
        {
            Offer? offer = sponsor.Offers.FirstOrDefault(o => o.Id == command.OfferId);
            offer?.SetMaxRedemptions(command.MaxRedemptions.Value);
        }

        Result result = sponsor.UpdateOffer(
            command.OfferId,
            command.Title,
            command.Description,
            moneyResult?.Value,
            command.DiscountPercentage,
            dateRangeResult.Value,
            command.TermsAndConditions);
        
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}