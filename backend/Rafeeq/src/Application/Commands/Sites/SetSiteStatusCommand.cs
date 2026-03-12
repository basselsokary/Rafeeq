using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;
using Domain.Enums;

namespace Application.Commands.Sites;

public record SetSiteStatusCommand(
    Guid Id,
    SiteStatus Status) : ICommand;

internal class SetSiteStatusCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<SetSiteStatusCommand>
{
    public async Task<Result> HandleAsync(SetSiteStatusCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetByIdAsync(command.Id, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.Id);
        
        site.SetStatus(command.Status);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}