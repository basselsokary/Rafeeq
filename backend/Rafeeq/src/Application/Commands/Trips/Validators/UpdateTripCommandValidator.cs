using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Common.Constants;
using Domain.Entities.TripAggregate;
using Domain.ValueObjects;
using FluentValidation;

namespace Application.Commands.Trips.Validators;

internal sealed class UpdateTripCommandValidator : AbstractValidator<UpdateTripCommand>
{
    public UpdateTripCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code]);

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(errors[TripErrors.TitleRequired.Code])
            .MaximumLength(DomainConstants.Trip.MaxNameLength)
            .WithMessage(errors[ValidationErrors.MaximumLengthExceeded.Code]);

        RuleFor(x => x.Description)
            .MaximumLength(DomainConstants.Trip.MaxDescriptionLength)
            .WithMessage(errors[ValidationErrors.MaximumLengthExceeded.Code])
            .When(x => x.Description != null);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-GeoLocation.BoundLatitude, GeoLocation.BoundLatitude)
            .WithMessage(x => errors.Format(GeoLocationErrors.InvalidLatitude(x.Latitude).Code, x.Latitude));

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-GeoLocation.BoundLongitude, GeoLocation.BoundLongitude)
            .WithMessage(x => errors.Format(GeoLocationErrors.InvalidLongitude(x.Longitude).Code, x.Longitude));

        RuleFor(x => x)
            .Must(x => x.StartDate.Date < x.EndDate.Date)
            .WithMessage(errors[DateRangeErrors.StartDateNotBeforeEndDate.Code]);

        RuleFor(x => x.EstimatedBudget)
            .GreaterThanOrEqualTo(0)
            .WithMessage(errors[MoneyErrors.NegativeAmount.Code])
            .When(x => x.EstimatedBudget.HasValue);
    }
}
