using Application.Queries.Common.Validators;
using FluentValidation;
using Application.Common.Interfaces.Localization;
using Domain.Common;

namespace Application.Queries.Users.Validators;

internal sealed class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code, x.Status.ToString()));

        RuleFor(x => x.Paging)
            .SetValidator(new PagingParametersValidator(errors))
            .WithMessage(errors[ValidationErrors.ValueRequired.Code])
            .When(x => x.Paging is not null);
    }
}
