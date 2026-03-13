using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;
using Domain.Enums;

namespace Application.Commands.Sites.LocalizedContents;

public record AddSiteLocalizedContentCommand(
    Guid Id,
    LanguageCode Language,
    string Name,
    string Description) : ICommand;

internal class AddSiteLocalizedContentCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<AddSiteLocalizedContentCommand>
{
    public async Task<Result> HandleAsync(AddSiteLocalizedContentCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetByIdAsync(command.Id, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.Id);

        Result result = site.AddLocalizedContent(command.Language, command.Name, command.Description);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

