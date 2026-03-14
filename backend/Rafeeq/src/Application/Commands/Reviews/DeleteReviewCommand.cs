using Domain.Common.Interfaces;
using Domain.Entities.ReviewAggregate;

namespace Application.Commands.Reviews;

public record DeleteReviewCommand(Guid Id) : ICommand;

internal class DeleteReviewCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteReviewCommand>
{
    public async Task<Result> HandleAsync(DeleteReviewCommand command, CancellationToken cancellationToken)
    {
        var review = await unitOfWork.Reviews.GetByIdAsync(command.Id, cancellationToken);
        if (review == null)
            return ReviewErrors.NotFound(command.Id);

        review.Delete();
        await unitOfWork.Reviews.DeleteAsync(review, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

