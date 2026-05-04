using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Commands.Sites.LocalizedContents;

public sealed record AddSiteLocalizedContentsCommand(
    Guid Id,
    List<AddSiteLocalizedContentsDtoCommand> LocalizedContents) : ICommand;

public sealed record AddSiteLocalizedContentsDtoCommand(
    LanguageCode Language,
    string Name,
    string Description,
    string Address,
    string? EntryFeeNotes);

internal sealed class AddSiteLocalizedContentCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<AddSiteLocalizedContentsCommand>
{
    public async Task<Result> HandleAsync(AddSiteLocalizedContentsCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetWithLocalizedContentsAsync(command.Id, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.Id);

        foreach (var content in command.LocalizedContents)
        {
            var addressResult = Address.Create(content.Address);
            if (addressResult.Failed)
                return addressResult;

            var result = site.AddLocalizedContent(content.Language, content.Name, content.Description, addressResult.Value, content.EntryFeeNotes);
            if (result.Failed)
                return result;
            
            await unitOfWork.AddAsync(result.Value, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

