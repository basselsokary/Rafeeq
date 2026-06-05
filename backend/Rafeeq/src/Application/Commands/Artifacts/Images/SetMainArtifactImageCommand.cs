using Domain.Common.Interfaces;
using Domain.Entities.ArtifactAggregate;

namespace Application.Commands.Artifacts.Images;

public sealed record SetMainArtifactImageCommand(
    Guid Id,
    Guid ImageId) : ICommand;

internal sealed class SetMainArtifactImageCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<SetMainArtifactImageCommand>
{
    public async Task<Result> HandleAsync(SetMainArtifactImageCommand command, CancellationToken cancellationToken)
    {
        var artifact = await unitOfWork.Artifacts.GetWithImagesAsync(command.Id, cancellationToken);
        if (artifact is null)
            return ArtifactErrors.NotFound(command.Id.ToString());

        var result = artifact.SetMainImage(command.ImageId);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
