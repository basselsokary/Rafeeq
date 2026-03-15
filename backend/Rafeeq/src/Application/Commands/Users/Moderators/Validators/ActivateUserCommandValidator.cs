using Domain.Entities.TouristAggregate;
using FluentValidation;

namespace Application.Commands.Users.Moderators.Validators;

public class ActivateUserCommandValidator : AbstractValidator<ActivateUserCommand>
{
    public ActivateUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(TouristErrors.RequiredUserId.Message);

    }
}
