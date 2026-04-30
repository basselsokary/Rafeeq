using Domain.Entities.SponsorAggregate;
using FluentValidation;
using Application.Common.Interfaces.Localization;

namespace Application.Commands.Sponsors.Validators;

internal sealed class DeleteSponsorCommandValidator : AbstractValidator<DeleteSponsorCommand>
{
    public DeleteSponsorCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.IdRequired.Code]);
    }
}

