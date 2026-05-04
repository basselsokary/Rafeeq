using Domain.Entities.SponsorAggregate;
using FluentValidation;
using Application.Common.Interfaces.Localization;

namespace Application.Commands.Sponsors.Validators;

internal sealed class ActivateSponsorCommandValidator : AbstractValidator<ActivateSponsorCommand>
{
    public ActivateSponsorCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.IdRequired.Code]);
    }
}

