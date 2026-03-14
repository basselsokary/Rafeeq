using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;

namespace Application.Commands.Sites.Images;

public record AddSiteImagesCommand(
    Guid Id,
    string ImageUrl,
    bool IsMain,
    string? Caption = null) : ICommand;

internal class AddSiteImagesCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<AddSiteImagesCommand>
{
    public async Task<Result> HandleAsync(AddSiteImagesCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetWithImagesAsync(command.Id, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.Id);

        Result result = site.AddImage(command.ImageUrl, command.IsMain, command.Caption);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}