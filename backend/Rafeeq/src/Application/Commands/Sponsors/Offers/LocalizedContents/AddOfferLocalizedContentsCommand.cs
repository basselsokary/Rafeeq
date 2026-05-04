using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;
using Domain.Enums;

namespace Application.Commands.Sponsors.Offers.LocalizedContents;

public sealed record AddOfferLocalizedContentsCommand(
    Guid OfferId,
    List<AddOfferLocalizedContentsDtoCommand> LocalizedContents) : ICommand;

public sealed record AddOfferLocalizedContentsDtoCommand(
    LanguageCode Language,
    string Title,
    string Description,
    string? TermsAndConditions);

internal sealed class AddOfferLocalizedContentCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<AddOfferLocalizedContentsCommand>
{
    public async Task<Result> HandleAsync(AddOfferLocalizedContentsCommand command, CancellationToken cancellationToken)
    {
        var offer = await unitOfWork.Sponsors.GetOfferByIdAsync(command.OfferId, cancellationToken);
        if (offer == null)
            return SponsorErrors.OfferNotFound(command.OfferId);

        foreach (var content in command.LocalizedContents)
        {
            Result<OfferLocalizedContent> result = offer.AddLocalizedContent(content.Language, content.Title, content.Description, content.TermsAndConditions);
            if (result.Failed)
                return result;
        
            await unitOfWork.AddAsync(result.Value, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
