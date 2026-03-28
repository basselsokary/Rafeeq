using Domain.Entities.SiteAggregate;
using Domain.ValueObjects;
using FluentValidation;

namespace Application.Commands.Sites.Validators;

internal class CreateSiteCommandValidator : AbstractValidator<CreateSiteCommand>
{
    public CreateSiteCommandValidator()
    {
        RuleFor(x => x.CityId)
            .NotEmpty()
            .WithMessage(SiteErrors.CityIdRequired.Message);
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(SiteErrors.NameRequired.Message);
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(SiteErrors.DescriptionRequired.Message);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-GeoLocation.BoundLatitude, GeoLocation.BoundLatitude)
            .WithMessage(GeoLocationErrors.InvalidLatitude(GeoLocation.BoundLatitude).Message);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-GeoLocation.BoundLongitude, GeoLocation.BoundLongitude)
            .WithMessage(GeoLocationErrors.InvalidLongitude(GeoLocation.BoundLongitude).Message);

        RuleFor(x => x.Type)
            .IsInEnum();
    }
}

internal class DeleteSiteCommandValidator : AbstractValidator<DeleteSiteCommand>
{
    public DeleteSiteCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SiteErrors.IdRequired.Message);
    }
}

internal class UpdateSiteCommandValidator : AbstractValidator<UpdateSiteCommand>
{
    public UpdateSiteCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SiteErrors.IdRequired.Message);
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(SiteErrors.NameRequired.Message);
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(SiteErrors.DescriptionRequired.Message);

        RuleFor(x => x.Type)
            .IsInEnum();

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-GeoLocation.BoundLatitude, GeoLocation.BoundLatitude)
            .WithMessage(GeoLocationErrors.InvalidLatitude(GeoLocation.BoundLatitude).Message);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-GeoLocation.BoundLongitude, GeoLocation.BoundLongitude)
            .WithMessage(GeoLocationErrors.InvalidLongitude(GeoLocation.BoundLongitude).Message);
    }
}

internal class SetSiteStatusCommandValidator : AbstractValidator<SetSiteStatusCommand>
{
    public SetSiteStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SiteErrors.IdRequired.Message);
        
        RuleFor(x => x.Status)
            .IsInEnum();
    }
}

internal class SetSiteContactInfoCommandValidator : AbstractValidator<SetSiteContactInfoCommand>
{
    public SetSiteContactInfoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SiteErrors.IdRequired.Message);
        
        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage(SiteErrors.PhoneRequired.Message);

        RuleFor(x => x.WebsiteUrl)
            .NotEmpty()
            .When(x => x.WebsiteUrl != null);
    }
}

internal class ActivateSiteCommandValidator : AbstractValidator<ActivateSiteCommand>
{
    public ActivateSiteCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SiteErrors.IdRequired.Message);
    }
}
