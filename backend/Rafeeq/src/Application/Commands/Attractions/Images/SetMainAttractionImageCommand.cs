using Domain.Common.Interfaces;
using Domain.Entities.AttractionAggregate;

namespace Application.Commands.Attractions.Images;

public sealed record SetMainAttractionImageCommand(
    Guid Id,
    Guid ImageId) : ICommand;

internal sealed class SetMainAttractionImageCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<SetMainAttractionImageCommand>
{
    public async Task<Result> HandleAsync(SetMainAttractionImageCommand command, CancellationToken cancellationToken)
    {
        var attraction = await unitOfWork.Attractions.GetWithImagesAsync(command.Id, cancellationToken);
        if (attraction is null)
            return AttractionErrors.NotFound(command.Id);

        attraction.SetMainImage(command.ImageId);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}