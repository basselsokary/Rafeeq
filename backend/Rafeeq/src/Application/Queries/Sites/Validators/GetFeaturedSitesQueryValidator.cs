using FluentValidation;
using Application.Common.Interfaces.Localization;
using Domain.Common;

namespace Application.Queries.Sites.Validators;

internal sealed class GetFeaturedSitesQueryValidator : AbstractValidator<GetFeaturedSitesQuery>
{
    public GetFeaturedSitesQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.City)
            .NotEmpty()
            .When(x => x.City.HasValue)
            .WithMessage(errors[ValidationErrors.ValueRequired.Code]);
    }
}

