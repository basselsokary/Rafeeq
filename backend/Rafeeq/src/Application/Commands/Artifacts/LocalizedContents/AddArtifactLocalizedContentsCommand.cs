using Domain.Common.Interfaces;
using Domain.Entities.ArtifactAggregate;
using Domain.Enums;

namespace Application.Commands.Artifacts.LocalizedContents;

public sealed record AddArtifactLocalizedContentsCommand(
    Guid Id,
    List<AddArtifactLocalizedContentsDtoCommand> LocalizedContents) : ICommand;

public sealed record AddArtifactLocalizedContentsDtoCommand(
    LanguageCode Language,
    string Name,
    string Description);

internal sealed class AddArtifactLocalizedContentsCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<AddArtifactLocalizedContentsCommand>
{
    public async Task<Result> HandleAsync(AddArtifactLocalizedContentsCommand command, CancellationToken cancellationToken)
    {
        var artifact = await unitOfWork.Artifacts.GetWithLocalizedContentsAsync(command.Id, cancellationToken);
        if (artifact == null)
            return ArtifactErrors.NotFound(command.Id.ToString());

        foreach (var content in command.LocalizedContents)
        {
            var result = artifact.AddLocalizedContent(content.Language, content.Name, content.Description);
            if (result.Failed)
                return result;

            await unitOfWork.AddAsync(result.Value, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
