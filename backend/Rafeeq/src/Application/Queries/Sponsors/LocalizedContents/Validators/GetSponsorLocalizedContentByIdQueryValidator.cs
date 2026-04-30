using Domain.Entities.SponsorAggregate;
using FluentValidation;
using Application.Common.Interfaces.Localization;

namespace Application.Queries.Sponsors.LocalizedContents.Validators;

internal sealed class GetSponsorLocalizedContentByIdQueryValidator : AbstractValidator<GetSponsorLocalizedContentByIdQuery>
{
    public GetSponsorLocalizedContentByIdQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.SponsorId)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.IdRequired.Code]);

        RuleFor(x => x.ContentId)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.ContentIdRequired.Code]);
    }
}
