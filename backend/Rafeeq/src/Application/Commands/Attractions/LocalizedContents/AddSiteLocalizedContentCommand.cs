using Domain.Common.Interfaces;
using Domain.Entities.AttractionAggregate;
using Domain.Enums;

namespace Application.Commands.Attractions;

public record AddAttractionLocalizedContentCommand(
    Guid Id,
    LanguageCode Language,
    string Name,
    string Description) : ICommand;

internal class AddAttractionLocalizedContentCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<AddAttractionLocalizedContentCommand>
{
    public async Task<Result> HandleAsync(AddAttractionLocalizedContentCommand command, CancellationToken cancellationToken)
    {
        var attraction = await unitOfWork.Attractions.GetWithLocalizedContentsAsync(command.Id, cancellationToken);
        if (attraction == null)
            return AttractionErrors.NotFound(command.Id);

        Result result = attraction.AddLocalizedContent(command.Language, command.Name, command.Description);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

