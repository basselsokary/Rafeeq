using Application.Common.Interfaces.Messaging;
using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;

namespace Application.Commands.Sites;

public record ActivateSiteCommand(
    Guid Id,
    bool Active) : ICommand;

internal class ActivateSiteCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<ActivateSiteCommand>
{
    public async Task<Result> HandleAsync(ActivateSiteCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetByIdAsync(command.Id, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.Id);
        
        site.Activate(command.Active);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}