using Application.Common.Interfaces.Messaging;
using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;

namespace Application.Commands.Sites;

public record DeleteSiteCommand(Guid Id) : ICommand;

internal class DeleteSiteCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteSiteCommand>
{
    public async Task<Result> HandleAsync(DeleteSiteCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetByIdAsync(command.Id, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.Id);

        var city = await unitOfWork.Cities.GetByIdAsync(site.CityId, cancellationToken);
        city?.DecrementSiteCount();

        await unitOfWork.Sites.DeleteAsync(site, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}


