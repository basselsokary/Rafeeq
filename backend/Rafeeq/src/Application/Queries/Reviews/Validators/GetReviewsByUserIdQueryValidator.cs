using FluentValidation;
using Application.Queries.Common.Validators;
using Application.Common.Interfaces.Localization;
using Domain.Common;

namespace Application.Queries.Reviews.Validators;

internal sealed class GetReviewsByUserIdQueryValidator : AbstractValidator<GetReviewsByUserIdQuery>
{
    public GetReviewsByUserIdQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Paging)
            .NotNull()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code])
            .SetValidator(new PagingParametersValidator(errors));
    }
}
