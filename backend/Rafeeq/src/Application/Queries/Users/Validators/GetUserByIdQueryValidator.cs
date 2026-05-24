using Application.Common.Interfaces.Localization;
using Domain.Common;
using FluentValidation;

namespace Application.Queries.Users.Validators;

internal sealed class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(errors[UserErrors.IdRequired.Code]);
    }
}
