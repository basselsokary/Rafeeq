using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Commands.Sites.NearestTransportations;

public sealed record AddNearestTransportationLocalizedContentCommand(
    Guid TransportationId,
    LanguageCode Language,
    string Name,
    string? Description,
    string Address) : ICommand;

internal sealed class AddNearestTransportationLocalizedContentCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<AddNearestTransportationLocalizedContentCommand>
{
    public async Task<Result> HandleAsync(AddNearestTransportationLocalizedContentCommand command, CancellationToken cancellationToken = default)
    {
        var transportation = await unitOfWork.Sites.GetNearestTransportationByIdAsync(command.TransportationId, cancellationToken);
        if (transportation is null)
            return SiteErrors.TransportationNotFound;

        var addressResult = Address.Create(command.Address);
        if (addressResult.Failed)
            return addressResult;

        var result = transportation.AddLocalizedContent(command.Language, command.Name, command.Description, addressResult.Value);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}