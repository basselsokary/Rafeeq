using Domain.Common.Interfaces;
using Domain.Entities.ArtifactAggregate;

namespace Application.Commands.Artifacts;

public sealed record DeleteArtifactCommand(Guid Id) : ICommand;

internal sealed class DeleteArtifactCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteArtifactCommand>
{
    public async Task<Result> HandleAsync(DeleteArtifactCommand command, CancellationToken cancellationToken)
    {
        var artifact = await unitOfWork.Artifacts.GetByIdAsync(command.Id, cancellationToken);
        if (artifact == null)
            return ArtifactErrors.NotFound(command.Id.ToString());

        await unitOfWork.Artifacts.DeleteAsync(artifact, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
