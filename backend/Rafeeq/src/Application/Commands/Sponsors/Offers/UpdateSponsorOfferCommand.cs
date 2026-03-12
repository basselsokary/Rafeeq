using Application.Common.Interfaces.Messaging;
using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;
using Domain.ValueObjects;

namespace Application.Commands.Sponsors.Offers;

public record UpdateSponsorOfferCommand(
    Guid SponsorId,
    Guid OfferId,
    string Title,
    string Description,
    Money? DiscountAmount,
    int? DiscountPercentage,
    DateRange ValidityPeriod,
    string? TermsAndConditions,
    int? MaxRedemptions) : ICommand;

internal class UpdateSponsorOfferCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateSponsorOfferCommand>
{
    public async Task<Result> HandleAsync(UpdateSponsorOfferCommand command, CancellationToken cancellationToken)
    {

        var sponsor = await unitOfWork.Sponsors.GetByIdAsync(command.SponsorId, cancellationToken);
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
            command.DiscountAmount,
            command.DiscountPercentage,
            command.TermsAndConditions);
        
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}