using FluentValidation;
using Application.Queries.Common.Validators;
using Application.Common.Interfaces.Localization;
using Domain.Common;

namespace Application.Queries.Sites.Validators;

internal sealed class GetSitesWithinBoundsQueryValidator : AbstractValidator<GetSitesWithinBoundsQuery>
{
    public GetSitesWithinBoundsQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Bounds)
            .NotNull()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code])
            .SetValidator(new BoundingBoxValidator(errors));

        RuleFor(x => x.Filters)
            .NotNull()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code])
            .SetValidator(new SiteFiltersValidator(errors));
    }
}

