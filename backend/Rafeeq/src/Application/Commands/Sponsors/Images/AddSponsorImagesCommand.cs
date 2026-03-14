using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;

namespace Application.Commands.Sponsors.Images;

public record AddSponsorImagesCommand(
    Guid Id,
    string ImageUrl,
    bool IsMain,
    string? Caption = null) : ICommand;

internal class AddSponsorImagesCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<AddSponsorImagesCommand>
{
    public async Task<Result> HandleAsync(AddSponsorImagesCommand command, CancellationToken cancellationToken)
    {
        var sponsor = await unitOfWork.Sponsors.GetWithImages(command.Id, cancellationToken);
        if (sponsor == null)
            return SponsorErrors.NotFound(command.Id);

        Result result = sponsor.AddImage(command.ImageUrl, command.IsMain, command.Caption);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}