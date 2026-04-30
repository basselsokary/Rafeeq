using Domain.Entities.CityAggregate;
using Domain.ValueObjects;
using FluentValidation;
using Application.Common.Interfaces.Localization;
using Domain.Common.Constants;

namespace Application.Commands.Cities.Validators;

internal sealed class UpdateCityCommandValidator : AbstractValidator<UpdateCityCommand>
{
    public UpdateCityCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[CityErrors.IdRequired.Code]);

        RuleFor(x => x.CenterLatitude)
            .InclusiveBetween(-GeoLocation.BoundLatitude, GeoLocation.BoundLatitude)
            .WithMessage(x =>
                errors.Format(GeoLocationErrors.InvalidLatitude(x.CenterLatitude).Code, x.CenterLatitude));

        RuleFor(x => x.CenterLongitude)
            .InclusiveBetween(-GeoLocation.BoundLongitude, GeoLocation.BoundLongitude)
            .WithMessage(x =>
                errors.Format(GeoLocationErrors.InvalidLongitude(x.CenterLongitude).Code, x.CenterLongitude));
        
        RuleFor(x => x.Image.Length)
            .GreaterThan(0)
            .LessThanOrEqualTo(DomainConstants.Image.MaxFileSizeBytes);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage(errors[CityErrors.NegativeDisplayOrder.Code]);
    }
}

