using FluentValidation;
using Domain.Entities.ReviewAggregate;
using Application.Common.Interfaces.Localization;

namespace Application.Queries.Reviews.Validators;

internal sealed class GetReviewByIdQueryValidator : AbstractValidator<GetReviewByIdQuery>
{
    public GetReviewByIdQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[ReviewErrors.IdRequired.Code]);
    }
}
