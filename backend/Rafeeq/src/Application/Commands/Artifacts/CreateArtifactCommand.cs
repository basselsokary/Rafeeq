using Domain.Common.Interfaces;
using Domain.Entities.ArtifactAggregate;
using Domain.Entities.SiteAggregate;
using Domain.Enums;

namespace Application.Commands.Artifacts;

public sealed record CreateArtifactCommand(
    Guid? SiteId,
    string Name,
    string Description,
    ArtifactType Type,
    int DisplayOrder) : ICommand<Guid>;

internal sealed class CreateArtifactCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<CreateArtifactCommand, Guid>
{
    public async Task<Result<Guid>> HandleAsync(CreateArtifactCommand command, CancellationToken cancellationToken)
    {
        if (command.SiteId.HasValue)
        {
            var siteExists = await unitOfWork.Sites.AnyAsync(command.SiteId.Value, cancellationToken);
            if (!siteExists)
                return SiteErrors.NotFound(command.SiteId.Value);
        }

        var artifactResult = Artifact.Create(
            command.SiteId,
            command.Name,
            command.Description,
            command.DisplayOrder,
            command.Type);

        if (artifactResult.Failed)
            return artifactResult.Error;

        await unitOfWork.Artifacts.AddAsync(artifactResult.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return artifactResult.Value.Id;
    }
}
