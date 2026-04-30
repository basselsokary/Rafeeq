using Domain.Common.Interfaces;
using Domain.Entities.AttractionAggregate;

namespace Application.Commands.Attractions;

public sealed record DeleteAttractionCommand(Guid Id) : ICommand;

internal sealed class DeleteAttractionCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteAttractionCommand>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> HandleAsync(DeleteAttractionCommand command, CancellationToken cancellationToken)
    {
        var attraction = await _unitOfWork.Attractions.GetByIdAsync(command.Id, cancellationToken);
        if (attraction == null)
            return AttractionErrors.NotFound(command.Id);
        
        await _unitOfWork.Attractions.DeleteAsync(attraction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}


