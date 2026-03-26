using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;
using Domain.Enums;

namespace Application.Commands.Sites.LocalizedContents;

public record AddSiteLocalizedContentsCommand(
    Guid Id,
    List<AddSiteLocalizedContentsDtoCommand> LocalizedContents) : ICommand;

public record AddSiteLocalizedContentsDtoCommand(
    LanguageCode Language,
    string Name,
    string Description);

internal class AddSiteLocalizedContentCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<AddSiteLocalizedContentsCommand>
{
    public async Task<Result> HandleAsync(AddSiteLocalizedContentsCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetWithLocalizedContentsAsync(command.Id, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.Id);

        foreach (var content in command.LocalizedContents)
        {
            Result result = site.AddLocalizedContent(content.Language, content.Name, content.Description);
            if (result.Failed)
                return result;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

