using Application.Common.Interfaces.Messaging;
using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;

namespace Application.Commands.Sponsors.Images;

public record RemoveSponsorImagesCommand(
    Guid Id,
    Guid ImageId) : ICommand;

internal class RemoveSponsorImagesCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveSponsorImagesCommand>
{
    public async Task<Result> HandleAsync(RemoveSponsorImagesCommand command, CancellationToken cancellationToken)
    {
        var sponsor = await unitOfWork.Sponsors.GetByIdAsync(command.Id, cancellationToken);
        if (sponsor == null)
            return SponsorErrors.NotFound(command.Id);

        Result result = sponsor.RemoveImage(command.ImageId);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}