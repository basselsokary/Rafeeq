using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Commands.Sponsors.LocalizedContents;

public sealed record AddSponsorLocalizedContentsCommand(
    Guid Id,
    List<AddSponsorLocalizedContentsDtoCommand> LocalizedContents) : ICommand;

public sealed record AddSponsorLocalizedContentsDtoCommand(
    LanguageCode Language,
    string Title,
    string Description,
    string Address);

internal sealed class AddSponsorLocalizedContentCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<AddSponsorLocalizedContentsCommand>
{
    public async Task<Result> HandleAsync(AddSponsorLocalizedContentsCommand command, CancellationToken cancellationToken)
    {
        var sponsor = await unitOfWork.Sponsors.GetWithLocalizedContentsAsync(command.Id, cancellationToken);
        if (sponsor == null)
            return SponsorErrors.NotFound(command.Id);

        foreach (var content in command.LocalizedContents)
        {
            var addressResult = Address.Create(content.Address);
            if (addressResult.Failed)
                return addressResult;
    
            var result = sponsor.AddLocalizedContent(content.Language, content.Title, content.Description, addressResult.Value);
            if (result.Failed)
                return result;
        
            await unitOfWork.AddAsync(result.Value, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
