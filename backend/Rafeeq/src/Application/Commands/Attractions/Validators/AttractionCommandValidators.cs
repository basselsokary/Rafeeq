using Domain.Entities.AttractionAggregate;
using Domain.ValueObjects;
using FluentValidation;
using static Domain.Common.Constants.DomainConstants.Attraction;

namespace Application.Commands.Attractions.Validators;

internal class CreateAttractionCommandValidator : AbstractValidator<CreateAttractionCommand>
{
    public CreateAttractionCommandValidator()
    {
        RuleFor(x => x.SiteId)
            .NotEmpty().WithMessage(AttractionErrors.SiteIdRequired.Message);

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(AttractionErrors.NameRequired.Message)
            .MaximumLength(MaxNameLength)
            .WithMessage(AttractionErrors.ExceededNameLength.Message);
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(AttractionErrors.DescriptionRequired.Message)
            .MaximumLength(MaxDescriptionLength)
            .WithMessage(AttractionErrors.ExceededDescriptionLength.Message);
        
        RuleFor(x => x.Type)
            .IsInEnum();

        RuleFor(x => x.HistoricalPeriod)
            .IsInEnum();
    }
}

internal class DeleteAttractionCommandValidator : AbstractValidator<DeleteAttractionCommand>
{
    public DeleteAttractionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(AttractionErrors.IdRequired.Message);
    }
}

internal class UpdateAttractionCommandValidator : AbstractValidator<UpdateAttractionCommand>
{
    public UpdateAttractionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(AttractionErrors.IdRequired.Message);

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(AttractionErrors.NameRequired.Message)
            .MaximumLength(MaxNameLength)
            .WithMessage(AttractionErrors.ExceededNameLength.Message);
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(AttractionErrors.DescriptionRequired.Message)
            .MaximumLength(MaxDescriptionLength)
            .WithMessage(AttractionErrors.ExceededDescriptionLength.Message);
        
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-GeoLocation.BoundLatitude, GeoLocation.BoundLatitude)
            .When(x => x.Latitude.HasValue)
            .WithMessage(GeoLocationErrors.InvalidLatitude(GeoLocation.BoundLatitude).Message);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-GeoLocation.BoundLongitude, GeoLocation.BoundLongitude)
            .When(x => x.Longitude.HasValue)
            .WithMessage(GeoLocationErrors.InvalidLongitude(GeoLocation.BoundLongitude).Message);
        
        RuleFor(x => x.Type)
            .IsInEnum();

        RuleFor(x => x.HistoricalPeriod)
            .IsInEnum();
    }
}

internal class MarkAttractionAsFeaturedCommandValidator : AbstractValidator<MarkAttractionAsFeaturedCommand>
{
    public MarkAttractionAsFeaturedCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(AttractionErrors.IdRequired.Message);
    }
}
