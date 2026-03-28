using Domain.Common.Interfaces;
using Domain.Entities.ReviewAggregate;
using Domain.ValueObjects;

namespace Application.Commands.Reviews;

public record UpdateReviewCommand(
    Guid Id,
    int Rating,
    string Title,
    string Content) : ICommand;

internal class UpdateReviewCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateReviewCommand>
{
    public async Task<Result> HandleAsync(UpdateReviewCommand command, CancellationToken cancellationToken)
    {
        var review = await unitOfWork.Reviews.GetByIdAsync(command.Id, cancellationToken);
        if (review == null)
            return ReviewErrors.NotFound(command.Id);

        var ratingResult = Rating.Create(command.Rating);
        if (ratingResult.Failed)
            return ratingResult;

        var result = review.Update(ratingResult.Value, command.Title, command.Content);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

