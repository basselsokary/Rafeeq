using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;
using Domain.ValueObjects;

namespace Application.Commands.Sites.NearestTransportations;

public sealed record UpdateNearestTransportationLocalizedContentCommand(
    Guid TransportationId,
    Guid ContentId,
    string Name,
    string? Description,
    string Address) : ICommand;

internal sealed class UpdateNearestTransportationLocalizedContentCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateNearestTransportationLocalizedContentCommand>
{
    public async Task<Result> HandleAsync(UpdateNearestTransportationLocalizedContentCommand command, CancellationToken cancellationToken = default)
    {
        var transportation = await unitOfWork.Sites.GetNearestTransportationByIdAsync(command.TransportationId, cancellationToken);
        if (transportation is null)
            return SiteErrors.TransportationNotFound;

        var addressResult = Address.Create(command.Address);
        if (addressResult.Failed)
            return addressResult;

        var result = transportation.UpdateLocalizedContent(command.ContentId, command.Name, command.Description, addressResult.Value);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}