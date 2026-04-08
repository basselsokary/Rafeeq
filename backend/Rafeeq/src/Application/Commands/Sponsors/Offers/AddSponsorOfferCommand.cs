using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;
using Domain.ValueObjects;

namespace Application.Commands.Sponsors.Offers;

public record AddSponsorOfferCommand(
    Guid SponsorId,
    string Title,
    string Description,
    decimal? DiscountAmount,
    int? DiscountPercentage,
    DateTime StartDate,
    DateTime EndDate,
    string? TermsAndConditions,
    int? MaxRedemptions) : ICommand;

internal class AddSponsorOfferCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<AddSponsorOfferCommand>
{
    public async Task<Result> HandleAsync(AddSponsorOfferCommand command, CancellationToken cancellationToken)
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

        Result result = sponsor.AddOffer(
            command.Title,
            command.Description,
            moneyResult?.Value,
            command.DiscountPercentage,
            dateRangeResult.Value,
            command.TermsAndConditions,
            command.MaxRedemptions);
        
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}