using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;

namespace Application.Commands.Sites.Images;

public record AddSiteImagesCommand(
    Guid Id,
    List<AddSiteImageDto> Images) : ICommand;

public record AddSiteImageDto(
    string ImageUrl,
    bool IsMain,
    int DisplayOrder,
    string? Caption = null);

internal class AddSiteImagesCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<AddSiteImagesCommand>
{
    public async Task<Result> HandleAsync(AddSiteImagesCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetWithImagesAsync(command.Id, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.Id);

        foreach (var image in command.Images)
        {
            Result result = site.AddImage(image.ImageUrl, image.IsMain, image.DisplayOrder, image.Caption);
            if (result.Failed)
                return result;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}