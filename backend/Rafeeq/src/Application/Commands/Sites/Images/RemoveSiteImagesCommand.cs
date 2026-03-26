using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;

namespace Application.Commands.Sites.Images;

public record RemoveSiteImagesCommand(
    Guid Id,
    List<Guid> ImageIds) : ICommand;

internal class RemoveSiteImagesCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveSiteImagesCommand>
{
    public async Task<Result> HandleAsync(RemoveSiteImagesCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetWithImagesAsync(command.Id, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.Id);

        foreach (var imageId in command.ImageIds)
        {
            Result result = site.RemoveImage(imageId);
            if (result.Failed)
                return result;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}