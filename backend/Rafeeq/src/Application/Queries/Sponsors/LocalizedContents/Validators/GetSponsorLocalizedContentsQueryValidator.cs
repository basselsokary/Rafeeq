using Domain.Entities.SponsorAggregate;
using FluentValidation;
using Application.Common.Interfaces.Localization;

namespace Application.Queries.Sponsors.LocalizedContents.Validators;

internal sealed class GetSponsorLocalizedContentsQueryValidator : AbstractValidator<GetSponsorLocalizedContentsQuery>
{
    public GetSponsorLocalizedContentsQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.SponsorId)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.IdRequired.Code]);
    }
}
