using Domain.Common.Interfaces;
using Domain.Entities.AttractionAggregate;

namespace Application.Commands.Attractions.LocalizedContents;

public record UpdateAttractionLocalizedContentCommand(
    Guid Id,
    Guid ContentId,
    string Name,
    string Description) : ICommand;

internal class UpdateAttractionLocalizedContentCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateAttractionLocalizedContentCommand>
{
    public async Task<Result> HandleAsync(UpdateAttractionLocalizedContentCommand command, CancellationToken cancellationToken)
    {
        var attraction = await unitOfWork.Attractions.GetWithLocalizedContentsAsync(command.Id, cancellationToken);
        if (attraction == null)
            return AttractionErrors.NotFound(command.Id);

        Result result = attraction.UpdateLocalizedContent(command.ContentId, command.Name, command.Description);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}