using Domain.Entities.SiteAggregate;
using Domain.ValueObjects;
using FluentValidation;
using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Common.Constants;

namespace Application.Commands.Sites.Validators;

internal sealed class UpdateSiteCommandValidator : AbstractValidator<UpdateSiteCommand>
{
    public UpdateSiteCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[SiteErrors.IdRequired.Code]);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-GeoLocation.BoundLatitude, GeoLocation.BoundLatitude)
            .WithMessage(x => errors.Format(GeoLocationErrors.InvalidLatitude(x.Latitude).Code, x.Latitude));

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-GeoLocation.BoundLongitude, GeoLocation.BoundLongitude)
            .WithMessage(x => errors.Format(GeoLocationErrors.InvalidLongitude(x.Longitude).Code, x.Longitude));

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code, x.Type.ToString()));
        
        RuleFor(x => x.EstimatedDurationMinutes)
            .GreaterThan(0)
            .WithMessage(errors[SiteErrors.InvalidEstimatedDuration.Code]);

        RuleFor(x => x.EgyptianTicketPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage(errors[MoneyErrors.NegativeAmount.Code]);

        RuleFor(x => x.ForeignerTicketPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage(errors[MoneyErrors.NegativeAmount.Code]);
        
        RuleFor(x => x.ForeignerCurrency)
            .NotEmpty()
            .When(x => x.ForeignerTicketPrice != null);
    }
}