using Application.Common.Interfaces.Localization;
using Domain.Common;
using FluentValidation;

namespace Application.Queries.Users.Validators;

internal sealed class SearchUsersQueryValidator : AbstractValidator<SearchUsersQuery>
{
    public SearchUsersQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.SearchTerm)
            .Must(term => !string.IsNullOrWhiteSpace(term))
            .WithMessage(errors[ValidationErrors.ValueRequired.Code]);

        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .WithMessage(errors[ValidationErrors.NumberMustBeGreaterThanZero.Code])
            .LessThanOrEqualTo(100)
            .WithMessage(errors[ValidationErrors.MaximumLengthExceeded.Code]);
    }
}
