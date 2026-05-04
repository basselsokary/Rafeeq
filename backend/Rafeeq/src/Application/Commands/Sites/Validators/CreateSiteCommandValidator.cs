using Domain.Entities.SiteAggregate;
using Domain.ValueObjects;
using FluentValidation;
using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Common.Constants;
using static Domain.Common.Constants.DomainConstants.Site;

namespace Application.Commands.Sites.Validators;

internal sealed class CreateSiteCommandValidator : AbstractValidator<CreateSiteCommand>
{
    public CreateSiteCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.CityId)
            .NotEmpty()
            .WithMessage(errors[SiteErrors.CityIdRequired.Code]);
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(errors[SiteErrors.NameRequired.Code])
            .MaximumLength(MaxNameLength)
            .WithMessage(errors.Format(SiteErrors.ExceededNameLength.Code, MaxNameLength));
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(errors[SiteErrors.DescriptionRequired.Code])
            .MaximumLength(MaxDescriptionLength)
            .WithMessage(errors.Format(SiteErrors.ExceededDescriptionLength.Code, MaxDescriptionLength));

        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage(errors[AddressErrors.EmptyAddress.Code])
            .MaximumLength(DomainConstants.Address.MaxAddressLength)
            .WithMessage(errors.Format(AddressErrors.ExceededAddressLength.Code, DomainConstants.Address.MaxAddressLength));

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
            .GreaterThanOrEqualTo(0)
            .WithMessage(errors[SiteErrors.InvalidEstimatedDuration.Code]);
        
        RuleFor(x => x.EgyptianTicketPrice)
            .GreaterThanOrEqualTo(0);
        
        RuleFor(x => x.ForeignerTicketPrice)
            .GreaterThanOrEqualTo(0);
    }
}