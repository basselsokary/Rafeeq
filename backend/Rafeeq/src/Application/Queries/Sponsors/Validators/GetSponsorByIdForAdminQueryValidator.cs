using Domain.Entities.SponsorAggregate;
using FluentValidation;
using Application.Common.Interfaces.Localization;

namespace Application.Queries.Sponsors.Validators;

internal sealed class GetSponsorByIdForAdminQueryValidator : AbstractValidator<GetSponsorByIdForAdminQuery>
{
    public GetSponsorByIdForAdminQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.IdRequired.Code]);
    }
}
