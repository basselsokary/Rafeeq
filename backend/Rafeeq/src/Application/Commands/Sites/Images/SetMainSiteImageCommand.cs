using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;

namespace Application.Commands.Sites.Images;

public sealed record SetMainSiteImageCommand(
    Guid Id,
    Guid ImageId) : ICommand;

internal sealed class SetMainSiteImageCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<SetMainSiteImageCommand>
{
    public async Task<Result> HandleAsync(SetMainSiteImageCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetWithImagesAsync(command.Id, cancellationToken);
        if (site is null)
            return SiteErrors.NotFound(command.Id);

        site.SetMainImage(command.ImageId);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}