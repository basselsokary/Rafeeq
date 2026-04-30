using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;

namespace Application.Commands.Sponsors.Offers.LocalizedContents;

public sealed record UpdateOfferLocalizedContentCommand(
    Guid OfferId,
    Guid ContentId,
    string Title,
    string Description,
    string? TermsAndConditions) : ICommand;

internal sealed class UpdateOfferLocalizedContentCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateOfferLocalizedContentCommand>
{
    public async Task<Result> HandleAsync(UpdateOfferLocalizedContentCommand command, CancellationToken cancellationToken)
    {
        var offer = await unitOfWork.Sponsors.GetOfferByIdAsync(command.OfferId, cancellationToken);
        if (offer == null)
            return SponsorErrors.OfferNotFound(command.OfferId);

        Result result = offer.UpdateLocalizedContent(command.ContentId, command.Title, command.Description, command.TermsAndConditions);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
