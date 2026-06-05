using Domain.Common.Interfaces;
using Domain.Entities.ArtifactAggregate;
using Domain.Enums;

namespace Application.Commands.Artifacts.LocalizedContents;

public sealed record UpdateArtifactLocalizedContentCommand(
    Guid Id,
    List<UpdateArtifactLocalizedContentsDtoCommand> LocalizedContents) : ICommand;

public sealed record UpdateArtifactLocalizedContentsDtoCommand(
    LanguageCode Language,
    string Name,
    string Description);

internal sealed class UpdateArtifactLocalizedContentCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateArtifactLocalizedContentCommand>
{
    public async Task<Result> HandleAsync(UpdateArtifactLocalizedContentCommand command, CancellationToken cancellationToken)
    {
        var artifact = await unitOfWork.Artifacts.GetWithLocalizedContentsAsync(command.Id, cancellationToken);
        if (artifact == null)
            return ArtifactErrors.NotFound(command.Id.ToString());

        foreach (var content in command.LocalizedContents)
        {
            var result = artifact.UpdateLocalizedContent(content.Language, content.Name, content.Description);
            if (result.Failed)
                return result;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
