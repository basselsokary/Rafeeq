using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;
using Domain.ValueObjects;

namespace Application.Commands.Sponsors;

public record SetSponsorContactInfoCommand(
    Guid Id,
    PhoneNumber Phone,
    Email Email,
    string? WebsiteUrl) : ICommand;

internal class SetSponsorContactInfoCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<SetSponsorContactInfoCommand>
{
    public async Task<Result> HandleAsync(SetSponsorContactInfoCommand command, CancellationToken cancellationToken)
    {
        var sponsor = await unitOfWork.Sponsors.GetByIdAsync(command.Id, cancellationToken);
        if (sponsor == null)
            return SponsorErrors.NotFound(command.Id);
        
        sponsor.SetContactInfo(command.Phone, command.Email, command.WebsiteUrl);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}