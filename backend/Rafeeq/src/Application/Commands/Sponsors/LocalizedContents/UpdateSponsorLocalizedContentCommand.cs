using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Commands.Sponsors.LocalizedContents;

public sealed record UpdateSponsorLocalizedContentCommand(
    Guid Id,
    List<UpdateSponsorLocalizedContentsDto> LocalizedContents) : ICommand;

public sealed record UpdateSponsorLocalizedContentsDto(
    LanguageCode Language,
    string Title,
    string Description,
    string Address);

internal sealed class UpdateSponsorLocalizedContentCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateSponsorLocalizedContentCommand>
{
    public async Task<Result> HandleAsync(UpdateSponsorLocalizedContentCommand command, CancellationToken cancellationToken)
    {
        var sponsor = await unitOfWork.Sponsors.GetWithLocalizedContentsAsync(command.Id, cancellationToken);
        if (sponsor == null)
            return SponsorErrors.NotFound(command.Id);


        foreach (var content in command.LocalizedContents)
        {
            var addressResult = Address.Create(content.Address);
            if (addressResult.Failed)
                return addressResult;

             var result = sponsor.UpdateLocalizedContent(content.Language, content.Title, content.Description, addressResult.Value);
            if (result.Failed)
                return result;
        }
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
