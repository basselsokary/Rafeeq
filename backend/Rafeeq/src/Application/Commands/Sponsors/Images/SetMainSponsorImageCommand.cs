using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;

namespace Application.Commands.Sponsors.Images;

public sealed record SetMainSponsorImageCommand(
    Guid Id,
    Guid ImageId) : ICommand;

internal sealed class SetMainSponsorImageCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<SetMainSponsorImageCommand>
{
    public async Task<Result> HandleAsync(SetMainSponsorImageCommand command, CancellationToken cancellationToken)
    {
        var sponsor = await unitOfWork.Sponsors.GetWithImagesAsync(command.Id, cancellationToken);
        if (sponsor is null)
            return SponsorErrors.NotFound(command.Id);

        sponsor.SetMainImage(command.ImageId);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}