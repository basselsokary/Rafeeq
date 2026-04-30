using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Common.Constants;
using Domain.Entities.SiteAggregate;
using Domain.ValueObjects;
using FluentValidation;

namespace Application.Commands.Sites.NearestTransportations.Validators;

internal sealed class AddSiteNearestTransportationCommandValidator : AbstractValidator<AddSiteNearestTransportationCommand>
{
    public AddSiteNearestTransportationCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.SiteId)
            .NotEmpty()
            .WithMessage(errors[SiteErrors.IdRequired.Code]);

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code, x.Type.ToString()));

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-GeoLocation.BoundLatitude, GeoLocation.BoundLatitude)
            .WithMessage(x => errors.Format(GeoLocationErrors.InvalidLatitude(x.Latitude).Code, x.Latitude));

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-GeoLocation.BoundLongitude, GeoLocation.BoundLongitude)
            .WithMessage(x => errors.Format(GeoLocationErrors.InvalidLongitude(x.Longitude).Code, x.Longitude));
        
        RuleFor(x => x.LocalizedContents)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.CollectionRequired.Code]);
        
        RuleForEach(x => x.LocalizedContents)
            .SetValidator(new AddNearestTransportationLocalizedContentDtoValidator(errors));
    }
}

internal sealed class AddNearestTransportationLocalizedContentDtoValidator : AbstractValidator<AddNearestTransportationLocalizedContentDto>
{
    public AddNearestTransportationLocalizedContentDtoValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Language)
            .IsInEnum()
            .WithMessage(errors[ValidationErrors.InvalidEnumValue.Code]);

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(errors[SiteErrors.NameRequired.Code])
            .MaximumLength(DomainConstants.Site.MaxNearestTransportationStationLength)
            .WithMessage(errors.Format(ValidationErrors.MaximumLengthExceeded.Code, DomainConstants.Site.MaxNearestTransportationStationLength));

        RuleFor(x => x.Description)
            .MaximumLength(DomainConstants.Site.MaxDescriptionLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Description))
            .WithMessage(errors.Format(ValidationErrors.MaximumLengthExceeded.Code, DomainConstants.Site.MaxDescriptionLength));

        RuleFor(x => x.Address)
            .MaximumLength(DomainConstants.Address.MaxAddressLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Address))
            .WithMessage(errors.Format(AddressErrors.ExceededAddressLength.Code, DomainConstants.Address.MaxAddressLength));
    }
}