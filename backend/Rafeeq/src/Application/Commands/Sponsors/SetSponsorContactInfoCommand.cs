using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;
using Domain.ValueObjects;

namespace Application.Commands.Sponsors;

public record SetSponsorContactInfoCommand(
    Guid Id,
    string Phone,
    string Email,
    string? WebsiteUrl) : ICommand;

internal class SetSponsorContactInfoCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<SetSponsorContactInfoCommand>
{
    public async Task<Result> HandleAsync(SetSponsorContactInfoCommand command, CancellationToken cancellationToken)
    {
        var phoneResult = PhoneNumber.Create(command.Phone);
		if (phoneResult.Failed)
			return phoneResult;

		var emailResult = Email.Create(command.Email);
		if (emailResult.Failed)
			return emailResult;

        var sponsor = await unitOfWork.Sponsors.GetByIdAsync(command.Id, cancellationToken);
        if (sponsor == null)
            return SponsorErrors.NotFound(command.Id);
        
        sponsor.SetContactInfo(phoneResult.Value, emailResult.Value, command.WebsiteUrl);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}