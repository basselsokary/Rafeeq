using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;

namespace Application.Commands.Sites;

public record SetSiteContactInfoCommand(
    Guid Id,
    string Phone,
    string? WebsiteUrl) : ICommand;

internal class SetSiteContactInfoCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<SetSiteContactInfoCommand>
{
    public async Task<Result> HandleAsync(SetSiteContactInfoCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetByIdAsync(command.Id, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.Id);
        
        Result result = site.SetContactInfo(command.Phone, command.WebsiteUrl);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}