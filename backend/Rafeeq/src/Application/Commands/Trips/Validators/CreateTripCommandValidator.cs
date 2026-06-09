using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Common.Constants;
using Domain.Entities.TripAggregate;
using Domain.Enums;
using Domain.ValueObjects;
using FluentValidation;

namespace Application.Commands.Trips.Validators;

internal sealed class CreateTripCommandValidator : AbstractValidator<CreateTripCommand>
{
    public CreateTripCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Title)
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
            .Must(x => x.StartDate <= x.EndDate)
            .WithMessage(errors[DateRangeErrors.StartDateNotBeforeEndDate.Code])
            .Must(x => (x.EndDate.DayNumber - x.StartDate.DayNumber) < DomainConstants.Trip.MaxDurationDays)
            .WithMessage(errors.Format(TripErrors.InvalidDurationDays(DomainConstants.Trip.MaxDurationDays).Code, DomainConstants.Trip.MaxDurationDays));

        RuleFor(x => x)
            .Must(x => x.DailyStartTime < x.DailyEndTime)
            .WithMessage(errors[TimeRangeErrors.StartTimeNotBeforeEndTime.Code]);

        RuleFor(x => x.EstimatedBudget)
            .GreaterThanOrEqualTo(0)
            .When(x => x.EstimatedBudget.HasValue)
            .WithMessage(errors[MoneyErrors.NegativeAmount.Code])
            .LessThanOrEqualTo(100_000)
            .When(x => x.EstimatedBudget.HasValue);
        
        RuleFor(x => x.Tolerance)
            .IsInEnum()
            .When(x => x.Tolerance.HasValue)
            .WithMessage(x => errors[ValidationErrors.InvalidEnumValue.Code]);
        
        RuleFor(x => x.PreferredSiteTypes)
            .Must(types => types.All(t => Enum.IsDefined(typeof(SiteType), t)))
            .WithMessage(errors[ValidationErrors.InvalidEnumValue.Code]);
    }
}
