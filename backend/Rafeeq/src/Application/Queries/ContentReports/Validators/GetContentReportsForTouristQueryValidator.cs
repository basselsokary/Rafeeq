using FluentValidation;
using Application.Queries.Common.Validators;
using Application.Common.Interfaces.Localization;
using Domain.Common;

namespace Application.Queries.ContentReports.Validators;

internal sealed class GetContentReportsForTouristQueryValidator : AbstractValidator<GetContentReportsForTouristQuery>
{
    public GetContentReportsForTouristQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Paging)
            .NotNull()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code])
            .SetValidator(new PagingParametersValidator(errors));

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue)
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code, x.Status.ToString() ?? "null"));
    }
}