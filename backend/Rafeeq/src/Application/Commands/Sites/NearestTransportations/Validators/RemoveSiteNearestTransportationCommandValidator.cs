using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Entities.SiteAggregate;
using FluentValidation;

namespace Application.Commands.Sites.NearestTransportations.Validators;

internal sealed class RemoveSiteNearestTransportationCommandValidator : AbstractValidator<RemoveSiteNearestTransportationCommand>
{
    public RemoveSiteNearestTransportationCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.SiteId)
            .NotEmpty()
            .WithMessage(errors[SiteErrors.IdRequired.Code]);

        RuleFor(x => x.TransportationId)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code]);
    }
}
