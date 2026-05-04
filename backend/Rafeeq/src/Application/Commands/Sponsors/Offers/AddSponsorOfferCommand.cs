using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Commands.Sponsors.Offers;

public sealed record AddSponsorOfferCommand(
    Guid SponsorId,
    decimal? DiscountAmount,
    int? DiscountPercentage,
    DateTime StartDate,
    DateTime EndDate,
    int? MaxRedemptions,
    string? PromoCode,
    List<AddOfferLocalizedContentDto> LocalizedContents) : ICommand;

public sealed record AddOfferLocalizedContentDto(LanguageCode Language, string Title, string Description, string? TermsAndConditions);

internal sealed class AddSponsorOfferCommandHandler(
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

        Result<Offer> offerResult = sponsor.AddOffer(
            moneyResult?.Value,
            command.DiscountPercentage,
            dateRangeResult.Value,
            command.MaxRedemptions,
            command.PromoCode);

        if (offerResult.Failed)
            return offerResult;
        
        var offer = offerResult.Value;
        
        foreach (var content in command.LocalizedContents)
        {
            var localizedContentResult = offer.AddLocalizedContent(
                content.Language,
                content.Title,
                content.Description,
                content.TermsAndConditions);

            if (localizedContentResult.Failed)
                return localizedContentResult;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}