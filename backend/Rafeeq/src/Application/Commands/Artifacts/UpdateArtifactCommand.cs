using Domain.Common.Interfaces;
using Domain.Entities.ArtifactAggregate;
using Domain.Entities.SiteAggregate;
using Domain.Enums;

namespace Application.Commands.Artifacts;

public sealed record UpdateArtifactCommand(
    Guid Id,
    int DisplayOrder,
    ArtifactType Type,
    Guid? SiteId) : ICommand;

internal sealed class UpdateArtifactCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateArtifactCommand>
{
    public async Task<Result> HandleAsync(UpdateArtifactCommand command, CancellationToken cancellationToken)
    {
        var artifact = await unitOfWork.Artifacts.GetByIdAsync(command.Id, cancellationToken);
        if (artifact == null)
            return ArtifactErrors.NotFound(command.Id.ToString());

        if (command.SiteId.HasValue)
        {
            var siteExists = await unitOfWork.Sites.AnyAsync(command.SiteId.Value, cancellationToken);
            if (!siteExists)
                return SiteErrors.NotFound(command.SiteId.Value);

            artifact.AssignSite(command.SiteId.Value);
        }

        var updateResult = artifact.Update(command.DisplayOrder, command.Type);
        if (updateResult.Failed)
            return updateResult;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
