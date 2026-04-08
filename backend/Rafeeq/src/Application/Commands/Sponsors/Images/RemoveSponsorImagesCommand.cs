using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;

namespace Application.Commands.Sponsors.Images;

public record RemoveSponsorImagesCommand(
    Guid Id,
    List<Guid> ImageIds) : ICommand;

internal class RemoveSponsorImagesCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveSponsorImagesCommand>
{
    public async Task<Result> HandleAsync(RemoveSponsorImagesCommand command, CancellationToken cancellationToken)
    {
        var sponsor = await unitOfWork.Sponsors.GetWithImagesAsync(command.Id, cancellationToken);
        if (sponsor == null)
            return SponsorErrors.NotFound(command.Id);

        foreach (var imageId in command.ImageIds)
        {
            Result result = sponsor.RemoveImage(imageId);
            if (result.Failed)
                return result;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}