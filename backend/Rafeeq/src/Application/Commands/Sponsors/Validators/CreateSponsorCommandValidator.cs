using Domain.Entities.SponsorAggregate;
using Domain.ValueObjects;
using FluentValidation;
using Application.Common.Interfaces.Localization;
using Domain.Common;
using static Domain.Common.Constants.DomainConstants.Sponsor;
using Domain.Common.Constants;
using System.Data;

namespace Application.Commands.Sponsors.Validators;

internal sealed class CreateSponsorCommandValidator : AbstractValidator<CreateSponsorCommand>
{
    public CreateSponsorCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-GeoLocation.BoundLatitude, GeoLocation.BoundLatitude)
            .WithMessage(x => errors.Format(GeoLocationErrors.InvalidLatitude(x.Latitude).Code, x.Latitude));

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-GeoLocation.BoundLongitude, GeoLocation.BoundLongitude)
            .WithMessage(x => errors.Format(GeoLocationErrors.InvalidLongitude(x.Longitude).Code, x.Longitude));

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code, x.Type.ToString()));
        
        RuleFor(x => x.Tier)
            .IsInEnum()
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code, x.Tier.ToString()));

        RuleFor(x => x.StartDate)
            .LessThan(x => x.EndDate)
            .WithMessage(errors[SponsorErrors.InvalidDate.Code]);
        
            RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.TitleRequired.Code])
            .MaximumLength(MaxTitleLength)
            .WithMessage(errors.Format(SponsorErrors.ExceededTitleLength.Code, MaxTitleLength));
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.DescriptionRequired.Code])
            .MaximumLength(MaxDescriptionLength)
            .WithMessage(errors.Format(SponsorErrors.ExceededDescriptionLength.Code, MaxDescriptionLength));
        
        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage(errors[AddressErrors.EmptyAddress.Code])
            .MaximumLength(DomainConstants.Address.MaxAddressLength)
            .WithMessage(errors.Format(AddressErrors.ExceededAddressLength.Code, DomainConstants.Address.MaxAddressLength));
    }
}
