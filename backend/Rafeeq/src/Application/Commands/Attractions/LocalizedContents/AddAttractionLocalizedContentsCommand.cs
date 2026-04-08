using Domain.Common.Interfaces;
using Domain.Entities.AttractionAggregate;
using Domain.Enums;

namespace Application.Commands.Attractions.LocalizedContents;

public record AddAttractionLocalizedContentsCommand(
    Guid Id,
    List<AddAttractionLocalizedContentsDtoCommand> LocalizedContents) : ICommand;

public record AddAttractionLocalizedContentsDtoCommand(
    LanguageCode Language,
    string Name,
    string Description);

internal class AddAttractionLocalizedContentsCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<AddAttractionLocalizedContentsCommand>
{
    public async Task<Result> HandleAsync(AddAttractionLocalizedContentsCommand command, CancellationToken cancellationToken)
    {
        var attraction = await unitOfWork.Attractions.GetWithLocalizedContentsAsync(command.Id, cancellationToken);
        if (attraction == null)
            return AttractionErrors.NotFound(command.Id);

        foreach (var content in command.LocalizedContents)
        {
            Result result = attraction.AddLocalizedContent(content.Language, content.Name, content.Description);
            if (result.Failed)
                return result;   
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

