using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;
using Domain.ValueObjects;

namespace Application.Commands.Sponsors.Offers;

public record AddSponsorOfferCommand(
    Guid SponsorId,
    string Title,
    string Description,
    Money? DiscountAmount,
    int? DiscountPercentage,
    DateRange ValidityPeriod,
    string? TermsAndConditions,
    int? MaxRedemptions) : ICommand;

internal class AddSponsorOfferCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<AddSponsorOfferCommand>
{
    public async Task<Result> HandleAsync(AddSponsorOfferCommand command, CancellationToken cancellationToken)
    {

        var sponsor = await unitOfWork.Sponsors.GetWithOffers(command.SponsorId, cancellationToken);
        if (sponsor == null)
            return SponsorErrors.NotFound(command.SponsorId);

        Result result = sponsor.AddOffer(
            command.Title,
            command.Description,
            command.DiscountAmount,
            command.DiscountPercentage,
            command.ValidityPeriod,
            command.TermsAndConditions,
            command.MaxRedemptions);
        
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}