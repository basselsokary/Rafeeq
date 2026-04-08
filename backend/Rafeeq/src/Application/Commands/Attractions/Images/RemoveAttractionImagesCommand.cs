using Domain.Common.Interfaces;
using Domain.Entities.AttractionAggregate;

namespace Application.Commands.Attractions.Images;

public record RemoveAttractionImagesCommand(
    Guid Id,
    List<Guid> ImageIds) : ICommand;

internal class RemoveImagesCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveAttractionImagesCommand>
{

    public async Task<Result> HandleAsync(RemoveAttractionImagesCommand command, CancellationToken cancellationToken)
    {
        var attraction = await unitOfWork.Attractions.GetWithImagesAsync(command.Id, cancellationToken);
        if (attraction == null)
            return AttractionErrors.NotFound(command.Id);

        foreach (var imageId in command.ImageIds)
        {
            Result result = attraction.RemoveImage(imageId);
            if (result.Failed)
                return result;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}