using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;

namespace Application.Commands.Sites.Images;

public record RemoveSiteImagesCommand(
    Guid Id,
    Guid ImageId) : ICommand;

internal class RemoveSiteImagesCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveSiteImagesCommand>
{
    public async Task<Result> HandleAsync(RemoveSiteImagesCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetByIdAsync(command.Id, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.Id);

        Result result = site.RemoveImage(command.ImageId);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}