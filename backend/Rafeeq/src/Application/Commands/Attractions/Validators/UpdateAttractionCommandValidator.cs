using Domain.Entities.AttractionAggregate;
using Domain.ValueObjects;
using FluentValidation;
using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Enums;

namespace Application.Commands.Attractions.Validators;

internal sealed class UpdateAttractionCommandValidator : AbstractValidator<UpdateAttractionCommand>
{
    public UpdateAttractionCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[AttractionErrors.IdRequired.Code]);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-GeoLocation.BoundLatitude, GeoLocation.BoundLatitude)
            .When(x => x.Latitude.HasValue)
            .WithMessage(x =>
                errors.Format(GeoLocationErrors.InvalidLatitude(x.Latitude ?? 0).Code, x.Latitude ?? 0));

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-GeoLocation.BoundLongitude, GeoLocation.BoundLongitude)
            .When(x => x.Longitude.HasValue)
            .WithMessage(x =>
                errors.Format(GeoLocationErrors.InvalidLongitude(x.Longitude ?? 0).Code, x.Longitude ?? 0));
        
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code, x.Type.ToString()));

        RuleFor(x => x.HistoricalPeriods)
            .NotNull()
            .WithMessage(errors[AttractionErrors.HistoricalPeriodRequired.Code])
            .Must(h => h.All(p => Enum.IsDefined(typeof(HistoricalPeriod), p)))
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code));
    }
}
