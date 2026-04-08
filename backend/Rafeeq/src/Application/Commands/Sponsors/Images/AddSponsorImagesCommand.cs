using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;

namespace Application.Commands.Sponsors.Images;

public record AddSponsorImagesCommand(
    Guid Id,
    List<AddSponsorImageDto> Images) : ICommand;

public record AddSponsorImageDto(
    string ImageUrl,
    bool IsMain,
    int DisplayOrder,
    string? Caption = null);

internal class AddSponsorImagesCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<AddSponsorImagesCommand>
{
    public async Task<Result> HandleAsync(AddSponsorImagesCommand command, CancellationToken cancellationToken)
    {
        var sponsor = await unitOfWork.Sponsors.GetWithImagesAsync(command.Id, cancellationToken);
        if (sponsor == null)
            return SponsorErrors.NotFound(command.Id);

        foreach (var image in command.Images)
        {
            Result result = sponsor.AddImage(image.ImageUrl, image.IsMain, image.DisplayOrder, image.Caption);
            if (result.Failed)
                return result;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}