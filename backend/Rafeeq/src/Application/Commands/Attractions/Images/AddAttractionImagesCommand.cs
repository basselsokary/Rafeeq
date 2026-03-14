using Domain.Common.Interfaces;
using Domain.Entities.AttractionAggregate;

namespace Application.Commands.Attractions;

public record AddAttractionImagesCommand(
    Guid Id,
    string ImageUrl,
    bool IsMain,
    string? Caption = null) : ICommand;

internal class AddAttractionImagesCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<AddAttractionImagesCommand>
{
    public async Task<Result> HandleAsync(AddAttractionImagesCommand command, CancellationToken cancellationToken)
    {
        var attraction = await unitOfWork.Attractions.GetWithImagesAsync(command.Id, cancellationToken);
        if (attraction == null)
            return AttractionErrors.NotFound(command.Id);

        Result result = attraction.AddImage(command.ImageUrl, command.IsMain, command.Caption);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}