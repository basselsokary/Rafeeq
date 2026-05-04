using FluentValidation;
using Application.Queries.Common.Validators;
using Application.Common.Interfaces.Localization;
using Domain.Common;

namespace Application.Queries.Reviews.Validators;

internal sealed class GetReviewsByStatusQueryValidator : AbstractValidator<GetReviewsByStatusQuery>
{
    public GetReviewsByStatusQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Paging)
            .NotNull()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code])
            .SetValidator(new PagingParametersValidator(errors));

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code, x.Status.ToString()));
    }
}
