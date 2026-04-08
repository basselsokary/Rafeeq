using Domain.Common.Interfaces;
using Domain.Entities.AttractionAggregate;

namespace Application.Commands.Attractions;

public record MarkAttractionAsFeaturedCommand(
    Guid Id,
    bool IsFeatured) : ICommand;

internal class MarkAttractionAsFeaturedCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<MarkAttractionAsFeaturedCommand>
{
    public async Task<Result> HandleAsync(MarkAttractionAsFeaturedCommand command, CancellationToken cancellationToken = default)
    {
        var attraction = await unitOfWork.Attractions.GetByIdAsync(command.Id, cancellationToken);
        if (attraction == null)
            return AttractionErrors.NotFound(command.Id);

        attraction.SetAsFeatured(command.IsFeatured);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}