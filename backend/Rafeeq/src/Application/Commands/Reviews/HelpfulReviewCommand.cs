using Domain.Common.Interfaces;
using Domain.Entities.ReviewAggregate;

namespace Application.Commands.Reviews;

public record HelpfulReviewCommand(
    Guid Id,
    bool IsHelpful) : ICommand;

internal class HelpfulReviewCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<HelpfulReviewCommand>
{
    public async Task<Result> HandleAsync(HelpfulReviewCommand command, CancellationToken cancellationToken)
    {
        var review = await unitOfWork.Reviews.GetByIdAsync(command.Id, cancellationToken);
        if (review == null)
            return ReviewErrors.NotFound(command.Id);
        
        if (command.IsHelpful)
            review.MarkAsHelpful();
        else
            review.MarkAsNotHelpful();
    
        return Result.Success();
    }
}