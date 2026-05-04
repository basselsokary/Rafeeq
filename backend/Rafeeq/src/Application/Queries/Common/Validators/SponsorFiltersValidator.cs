using Application.DTOs.Sponsors;
using FluentValidation;
using Application.Common.Interfaces.Localization;
using Domain.Common;

namespace Application.Queries.Common.Validators;

internal sealed class SponsorFiltersValidator : AbstractValidator<SponsorFilters>
{
    public SponsorFiltersValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .When(x => x.Type.HasValue)
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code, x.Type.ToString() ?? "null"));

        RuleFor(x => x.Tier)
            .IsInEnum()
            .When(x => x.Tier.HasValue)
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code, x.Tier.ToString() ?? "null" ));
    }
}
