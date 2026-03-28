using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;

namespace Application.Commands.Sites;

public record MarkSiteAsFeaturedCommand(
    Guid Id,
    bool IsFeatured) : ICommand;

internal class MarkSiteAsFeaturedCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<MarkSiteAsFeaturedCommand>
{
    public async Task<Result> HandleAsync(MarkSiteAsFeaturedCommand command, CancellationToken cancellationToken = default)
    {
        var site = await unitOfWork.Sites.GetByIdAsync(command.Id, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.Id);

        site.SetAsFeatured(command.IsFeatured);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}