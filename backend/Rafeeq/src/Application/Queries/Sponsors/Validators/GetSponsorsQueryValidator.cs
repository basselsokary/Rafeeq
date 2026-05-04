using Application.Queries.Common.Validators;
using FluentValidation;
using Application.Common.Interfaces.Localization;
using Domain.Common;

namespace Application.Queries.Sponsors.Validators;

internal sealed class GetSponsorsQueryValidator : AbstractValidator<GetSponsorsQuery>
{
    public GetSponsorsQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Filters)
            .NotNull()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code])
            .SetValidator(new SponsorFiltersValidator(errors));

        RuleFor(x => x.Paging)
            .NotNull()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code])
            .SetValidator(new PagingParametersValidator(errors));
    }
}
