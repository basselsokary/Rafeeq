using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Commands.Sites.LocalizedContents;

public sealed record UpdateSiteLocalizedContentCommand(
    Guid Id,
    List<UpdateSiteLocalizedContentsDto> LocalizedContents) : ICommand;

public sealed record UpdateSiteLocalizedContentsDto(
    LanguageCode Language,
    string Name,
    string Description,
    string Address,
    string? EntryFeeNotes);

internal sealed class UpdateSiteLocalizedContentCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateSiteLocalizedContentCommand>
{
    public async Task<Result> HandleAsync(UpdateSiteLocalizedContentCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetWithLocalizedContentsAsync(command.Id, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.Id);
        
        foreach (var content in command.LocalizedContents)
        {
            var addressResult = Address.Create(content.Address);
            if (addressResult.Failed)
                return addressResult;

            Result result = site.UpdateLocalizedContent(content.Language, content.Name, content.Description, addressResult.Value, content.EntryFeeNotes);
            if (result.Failed)
                return result;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}