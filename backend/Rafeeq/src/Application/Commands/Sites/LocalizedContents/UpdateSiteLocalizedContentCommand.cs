using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;

namespace Application.Commands.Sites;

public record UpdateSiteLocalizedContentCommand(
    Guid Id,
    Guid ContentId,
    string Name,
    string Description) : ICommand;

internal class UpdateSiteLocalizedContentCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateSiteLocalizedContentCommand>
{
    public async Task<Result> HandleAsync(UpdateSiteLocalizedContentCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetWithLocalizedContentsAsync(command.Id, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.Id);

        Result result = site.UpdateLocalizedContent(command.ContentId, command.Name, command.Description);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}