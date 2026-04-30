using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Entities.SiteAggregate;
using Domain.ValueObjects;
using FluentValidation;

namespace Application.Commands.Sites.NearestTransportations.Validators;

internal sealed class UpdateSiteNearestTransportationCommandValidator : AbstractValidator<UpdateSiteNearestTransportationCommand>
{
    public UpdateSiteNearestTransportationCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.SiteId)
            .NotEmpty()
            .WithMessage(errors[SiteErrors.IdRequired.Code]);

        RuleFor(x => x.TransportationId)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code]);

        RuleFor(x => x)
            .Must(x => x.StartOperatingHoursTime < x.EndOperatingHoursTime)
            .WithMessage(errors[TimeRangeErrors.StartTimeNotBeforeEndTime.Code]);
    }
}
