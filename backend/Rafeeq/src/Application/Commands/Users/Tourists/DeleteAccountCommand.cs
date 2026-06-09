using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Localization;
using Domain.Common.Interfaces;
using Domain.Entities.TouristAggregate;

namespace Application.Commands.Users.Tourists;

public sealed record DeleteAccountCommand : ICommand;

internal sealed class DeleteAccountCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IErrorLocalizer errorLocalizer) : ICommandHandler<DeleteAccountCommand>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> HandleAsync(DeleteAccountCommand command, CancellationToken cancellationToken = default)
    {
        var tourist = await _unitOfWork.Tourists.GetByIdAsync(userContext.Id, cancellationToken);
        if (tourist is null)
            return Result.Failure(errorLocalizer[TouristErrors.NotFound(userContext.Id).Code]);
        
        tourist.Delete();

        await _unitOfWork.Tourists.DeleteAsync(tourist, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}