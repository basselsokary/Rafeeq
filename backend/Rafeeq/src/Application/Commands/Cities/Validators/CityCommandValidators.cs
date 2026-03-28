using Domain.Entities.CityAggregate;
using Domain.ValueObjects;
using FluentValidation;
using static Domain.Common.Constants.DomainConstants.City;

namespace Application.Commands.Cities.Validators;

internal class CreateCityCommandValidator : AbstractValidator<CreateCityCommand>
{
    public CreateCityCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(CityErrors.NameRequired.Message)
            .MaximumLength(MaxNameLength)
            .WithMessage(CityErrors.ExceededNameLength.Message);
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(CityErrors.DescriptionRequired.Message)
            .MaximumLength(MaxDescriptionLength)
            .WithMessage(CityErrors.ExceededDescriptionLength.Message);

        RuleFor(x => x.CenterLatitude)
            .InclusiveBetween(-GeoLocation.BoundLatitude, GeoLocation.BoundLatitude)
            .WithMessage(GeoLocationErrors.InvalidLatitude(GeoLocation.BoundLatitude).Message);

        RuleFor(x => x.CenterLongitude)
            .InclusiveBetween(-GeoLocation.BoundLongitude, GeoLocation.BoundLongitude)
            .WithMessage(GeoLocationErrors.InvalidLongitude(GeoLocation.BoundLongitude).Message);
    }
}

internal class DeleteCityCommandValidator : AbstractValidator<DeleteCityCommand>
{
    public DeleteCityCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(CityErrors.IdRequired.Message);
    }
}

internal class UpdateCityCommandValidator : AbstractValidator<UpdateCityCommand>
{
    public UpdateCityCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(CityErrors.IdRequired.Message);

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(CityErrors.NameRequired.Message)
            .MaximumLength(MaxNameLength)
            .WithMessage(CityErrors.ExceededNameLength.Message);
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(CityErrors.DescriptionRequired.Message)
            .MaximumLength(MaxDescriptionLength)
            .WithMessage(CityErrors.ExceededDescriptionLength.Message);

        RuleFor(x => x.CenterLatitude)
            .InclusiveBetween(-GeoLocation.BoundLatitude, GeoLocation.BoundLatitude)
            .WithMessage(GeoLocationErrors.InvalidLatitude(GeoLocation.BoundLatitude).Message);

        RuleFor(x => x.CenterLongitude)
            .InclusiveBetween(-GeoLocation.BoundLongitude, GeoLocation.BoundLongitude)
            .WithMessage(GeoLocationErrors.InvalidLongitude(GeoLocation.BoundLongitude).Message);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage(CityErrors.NegativeDisplayOrder.Message);
    }
}

