using Application.Common.Interfaces.Messaging;
using Domain.Common.Interfaces;
using Domain.Entities.AttractionAggregate;

namespace Application.Commands.Attractions;

public record RemoveAttractionImagesCommand(
    Guid Id,
    Guid ImageId) : ICommand;

internal class RemoveImagesCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveAttractionImagesCommand>
{

    public async Task<Result> HandleAsync(RemoveAttractionImagesCommand command, CancellationToken cancellationToken)
    {
        var attraction = await unitOfWork.Attractions.GetByIdAsync(command.Id, cancellationToken);
        if (attraction == null)
            return AttractionErrors.NotFound(command.Id);

        Result result = attraction.RemoveImage(command.ImageId);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}