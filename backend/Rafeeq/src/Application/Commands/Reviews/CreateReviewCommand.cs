using Application.Common.Interfaces.Authentication;
using Domain.Common.Interfaces;
using Domain.Entities.ReviewAggregate;
using Domain.Entities.SiteAggregate;
using Domain.ValueObjects;

namespace Application.Commands.Reviews;

public sealed record CreateReviewCommand(
    Guid SiteId,
    int Rating,
    string Title,
    string Content) : ICommand;

internal sealed class CreateReviewCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext) : ICommandHandler<CreateReviewCommand>
{
    public async Task<Result> HandleAsync(CreateReviewCommand command, CancellationToken cancellationToken)
    {
        var ratingResult = Rating.Create(command.Rating);
        if (ratingResult.Failed)
            return ratingResult;
        
        var reviewResult = Review.Create(
            userContext.Id,
            command.SiteId,
            ratingResult.Value,
            command.Title,
            command.Content);

        if (reviewResult.Failed)
            return reviewResult;

        var siteExist = await unitOfWork.Sites.AnyAsync(command.SiteId, cancellationToken);
        if (!siteExist)
            return SiteErrors.NotFound(command.SiteId);
        
        bool HasUserReviewedSite = await unitOfWork.Reviews.HasUserReviewedSiteAsync(
            userContext.Id,
            command.SiteId,
            cancellationToken);

        if (HasUserReviewedSite)
            return ReviewErrors.UserAlreadyReviewdSite;

        await unitOfWork.Reviews.AddAsync(reviewResult.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}
