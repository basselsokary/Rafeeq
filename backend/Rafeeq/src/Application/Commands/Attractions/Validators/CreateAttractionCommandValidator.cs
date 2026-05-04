using Domain.Entities.AttractionAggregate;
using FluentValidation;
using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;
using static Domain.Common.Constants.DomainConstants.Attraction;

namespace Application.Commands.Attractions.Validators;

internal sealed class CreateAttractionCommandValidator : AbstractValidator<CreateAttractionCommand>
{
    public CreateAttractionCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.SiteId)
            .NotEmpty()
            .WithMessage(errors[AttractionErrors.SiteIdRequired.Code]);
        
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code, x.Type.ToString()));
        
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

        RuleFor(x => x.HistoricalPeriod)
            .NotNull()
            .WithMessage(errors[AttractionErrors.HistoricalPeriodRequired.Code])
            .Must(h => h.All(p => Enum.IsDefined(typeof(HistoricalPeriod), p)))
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code));

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(errors[AttractionErrors.NameRequired.Code])
            .MaximumLength(MaxNameLength)
            .WithMessage(errors.Format(AttractionErrors.ExceededNameLength.Code, MaxNameLength));
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(errors[AttractionErrors.DescriptionRequired.Code])
            .MaximumLength(MaxDescriptionLength)
            .WithMessage(errors.Format(AttractionErrors.ExceededDescriptionLength.Code, MaxDescriptionLength));
        
        RuleFor(x => x.LocationDescription)
            .MaximumLength(MaxLocationDescriptionLength)
            .When(x => !string.IsNullOrEmpty(x.LocationDescription))
            .WithMessage(errors.Format(AttractionErrors.ExceededLocationDescriptionLength.Code, MaxLocationDescriptionLength));
    }
}
