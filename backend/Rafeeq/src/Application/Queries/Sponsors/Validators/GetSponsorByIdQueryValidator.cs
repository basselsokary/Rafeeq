using Domain.Entities.SponsorAggregate;
using FluentValidation;
using Application.Common.Interfaces.Localization;

namespace Application.Queries.Sponsors.Validators;

internal sealed class GetSponsorByIdQueryValidator : AbstractValidator<GetSponsorByIdQuery>
{
    public GetSponsorByIdQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.IdRequired.Code]);
    }
}
