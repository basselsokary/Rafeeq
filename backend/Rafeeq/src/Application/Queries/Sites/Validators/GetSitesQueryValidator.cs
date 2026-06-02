using FluentValidation;
using Application.Queries.Common.Validators;
using Application.Common.Interfaces.Localization;
using Domain.Common;

namespace Application.Queries.Sites.Validators;

internal sealed class GetSitesQueryValidator : AbstractValidator<GetSitesQuery>
{
    public GetSitesQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Filters)
            .SetValidator(new SiteFiltersValidator(errors))
            .WithMessage(errors[ValidationErrors.ValueRequired.Code])
            .When(x => x.Filters is not null);

        RuleFor(x => x.Paging)
            .SetValidator(new PagingParametersValidator(errors))
            .WithMessage(errors[ValidationErrors.ValueRequired.Code])
            .When(x => x.Paging is not null);

        RuleFor(x => x.SearchTerm)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.SearchTerm))
            .WithMessage(errors.Format(ValidationErrors.MaximumLengthExceeded.Code));
    }
}

