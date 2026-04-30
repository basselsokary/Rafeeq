using Domain.Common.Interfaces;
using Domain.Entities.AttractionAggregate;
using Domain.Enums;

namespace Application.Commands.Attractions.LocalizedContents;

public sealed record UpdateAttractionLocalizedContentCommand(
    Guid Id,
    List<UpdateAttractionLocalizedContentsDtoCommand> LocalizedContents) : ICommand;

public sealed record UpdateAttractionLocalizedContentsDtoCommand(
    LanguageCode Language,
    string Name,
    string Description,
    string? LocationDescription);

internal sealed class UpdateAttractionLocalizedContentCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateAttractionLocalizedContentCommand>
{
    public async Task<Result> HandleAsync(UpdateAttractionLocalizedContentCommand command, CancellationToken cancellationToken)
    {
        var attraction = await unitOfWork.Attractions.GetWithLocalizedContentsAsync(command.Id, cancellationToken);
        if (attraction == null)
            return AttractionErrors.NotFound(command.Id);

        foreach (var content in command.LocalizedContents)
        {
            Result result = attraction.UpdateLocalizedContent(content.Language, content.Name, content.Description, content.LocationDescription);
            if (result.Failed)
                return result;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}