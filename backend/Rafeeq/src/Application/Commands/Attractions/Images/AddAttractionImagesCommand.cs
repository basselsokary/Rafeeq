using Domain.Common.Interfaces;
using Domain.Entities.AttractionAggregate;

namespace Application.Commands.Attractions.Images;

public record AddAttractionImagesCommand(
    Guid Id,
    List<AddAttractionImageDto> Images) : ICommand;

public record AddAttractionImageDto(
    string ImageUrl,
    bool IsMain,
    int DisplayOrder,
    string? Caption = null);

internal class AddAttractionImagesCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<AddAttractionImagesCommand>
{
    public async Task<Result> HandleAsync(AddAttractionImagesCommand command, CancellationToken cancellationToken)
    {
        var attraction = await unitOfWork.Attractions.GetWithImagesAsync(command.Id, cancellationToken);
        if (attraction == null)
            return AttractionErrors.NotFound(command.Id);

        foreach (var image in command.Images)
        {
            Result result = attraction.AddImage(image.ImageUrl, image.IsMain, image.DisplayOrder, image.Caption);
            if (result.Failed)
                return result;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}