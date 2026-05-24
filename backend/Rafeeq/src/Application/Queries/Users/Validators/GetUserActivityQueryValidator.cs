using Application.Common.Interfaces.Localization;
using Domain.Common;
using FluentValidation;

namespace Application.Queries.Users.Validators;

internal sealed class GetUserActivityQueryValidator : AbstractValidator<GetUserActivityQuery>
{
    public GetUserActivityQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(errors[UserErrors.IdRequired.Code]);

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage(errors[ValidationErrors.NumberMustBeGreaterThanZero.Code]);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage(errors[ValidationErrors.NumberMustBeGreaterThanZero.Code])
            .LessThanOrEqualTo(100)
            .WithMessage(errors[ValidationErrors.MaximumLengthExceeded.Code]);
    }
}
