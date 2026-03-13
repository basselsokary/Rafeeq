using Domain.Common.Interfaces;
using Domain.Entities.ReviewAggregate;
using Domain.Enums;

namespace Application.Commands.Reviews;

public record SetReviewStatusCommand(
    Guid Id,
    ReviewStatus Status,
    string RejectionReason) : ICommand;

internal class SetReviewStatusCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<SetReviewStatusCommand>
{
    public async Task<Result> HandleAsync(SetReviewStatusCommand command, CancellationToken cancellationToken)
    {
        var review = await unitOfWork.Reviews.GetByIdAsync(command.Id, cancellationToken);
        if (review == null)
            return ReviewErrors.NotFound(command.Id);
        
        var result = command.Status switch
        {
            ReviewStatus.Approved => review.Approve(),
            ReviewStatus.Flagged => review.Flag(),
            ReviewStatus.Rejected => review.Reject(command.RejectionReason),
            _ => Result.Failure(ReviewErrors.InvalidStatus(command.Status))
        };

        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}