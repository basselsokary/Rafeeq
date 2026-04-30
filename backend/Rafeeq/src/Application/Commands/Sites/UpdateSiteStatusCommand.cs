using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;
using Domain.Enums;

namespace Application.Commands.Sites;

public sealed record UpdateSiteStatusCommand(
    Guid Id,
    SiteStatus Status,
    bool IsFeatured,
    bool IsHiddenGem) : ICommand;

internal sealed class UpdateSiteStatusCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateSiteStatusCommand>
{
    public async Task<Result> HandleAsync(UpdateSiteStatusCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetByIdAsync(command.Id, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.Id);

        var result = site.UpdateStatus(command.Status, command.IsHiddenGem, command.IsFeatured);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}