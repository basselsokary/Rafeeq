using Domain.Common.Interfaces;
using Domain.Entities.AttractionAggregate;
using Domain.Enums;

namespace Application.Commands.Attractions.LocalizedContents;

public sealed record AddAttractionLocalizedContentsCommand(
    Guid Id,
    List<AddAttractionLocalizedContentsDtoCommand> LocalizedContents) : ICommand;

public sealed record AddAttractionLocalizedContentsDtoCommand(
    LanguageCode Language,
    string Name,
    string Description,
    string? LocationDescription);

internal sealed class AddAttractionLocalizedContentsCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<AddAttractionLocalizedContentsCommand>
{
    public async Task<Result> HandleAsync(AddAttractionLocalizedContentsCommand command, CancellationToken cancellationToken)
    {
        var attraction = await unitOfWork.Attractions.GetWithLocalizedContentsAsync(command.Id, cancellationToken);
        if (attraction == null)
            return AttractionErrors.NotFound(command.Id);

        foreach (var content in command.LocalizedContents)
        {
            var result = attraction.AddLocalizedContent(content.Language, content.Name, content.Description, content.LocationDescription);
            if (result.Failed)
                return result;
                
            await unitOfWork.AddAsync(result.Value, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

