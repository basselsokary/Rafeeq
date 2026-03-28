using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;
using Domain.ValueObjects;

namespace Application.Commands.Sponsors.Offers;

public record ActivateSponsorOfferCommand(
    Guid SponsorId,
    Guid OfferId,
    bool IsActive,
    DateTime? StartDate = null,
    DateTime? EndDate = null) : ICommand;

internal class ActivateSponsorOfferCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<ActivateSponsorOfferCommand>
{
    public async Task<Result> HandleAsync(ActivateSponsorOfferCommand command, CancellationToken cancellationToken)
    {
        var sponsor = await unitOfWork.Sponsors.GetWithOffersAsync(command.SponsorId, cancellationToken);
        if (sponsor == null)
            return SponsorErrors.NotFound(command.SponsorId);

        var offer = sponsor.Offers.FirstOrDefault(o => o.Id == command.OfferId);
        if (offer == null)
            return SponsorErrors.OfferNotFound(command.OfferId);
        
        if (command.IsActive)
        {
            Result result;
            if (command.StartDate.HasValue && command.EndDate.HasValue)
            {
                var validityPeriodResult = DateRange.Create(command.StartDate.Value, command.EndDate.Value);
                if (validityPeriodResult.Failed)
                    return validityPeriodResult;
                
                result = offer.Activate(validityPeriodResult.Value);
            } else
            {
                result = offer.Activate();
            }

            if (result.Failed)
                return result;
        } else
        {
            offer.Deactivate();
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}