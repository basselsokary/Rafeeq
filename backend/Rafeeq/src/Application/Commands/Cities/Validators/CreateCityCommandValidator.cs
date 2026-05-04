using Domain.Entities.CityAggregate;
using Domain.ValueObjects;
using FluentValidation;
using Application.Common.Interfaces.Localization;
using Application.Common.Validators;
using Domain.Common.Constants;
using static Domain.Common.Constants.DomainConstants.City;

namespace Application.Commands.Cities.Validators;

internal sealed class CreateCityCommandValidator : AbstractValidator<CreateCityCommand>
{
    public CreateCityCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.OriginalFileName)
            .NotEmpty()
            .WithMessage(errors[CityErrors.ImageUrlRequired.Code])
            .MaximumLength(DomainConstants.Image.MaxImageUrlLength);

        RuleFor(x => x.Image.Length)
            .GreaterThan(0)
            .LessThanOrEqualTo(DomainConstants.Image.MaxFileSizeBytes);
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(errors[CityErrors.NameRequired.Code])
            .MaximumLength(MaxNameLength)
            .WithMessage(errors.Format(CityErrors.ExceededNameLength.Code, MaxNameLength));
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(errors[CityErrors.DescriptionRequired.Code])
            .MaximumLength(MaxDescriptionLength)
            .WithMessage(errors.Format(CityErrors.ExceededDescriptionLength.Code, MaxDescriptionLength));
        
        RuleFor(x => x)
            .Must(x =>
            {
                var ext = Path.GetExtension(x.OriginalFileName);
                return FileSignatureValidator.IsValid(x.Image, ext);
            });
            
        RuleFor(x => x.CenterLatitude)
            .InclusiveBetween(-GeoLocation.BoundLatitude, GeoLocation.BoundLatitude)
            .WithMessage(x =>
                errors.Format(GeoLocationErrors.InvalidLatitude(x.CenterLatitude).Code, x.CenterLatitude));

        RuleFor(x => x.CenterLongitude)
            .InclusiveBetween(-GeoLocation.BoundLongitude, GeoLocation.BoundLongitude)
            .WithMessage(x =>
                errors.Format(GeoLocationErrors.InvalidLongitude(x.CenterLongitude).Code, x.CenterLongitude));
            
        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}