using Domain.Entities.CityAggregate;
using Domain.ValueObjects;
using FluentValidation;
using Application.Common.Interfaces.Localization;
using Application.Common.Validators;
using Domain.Common.Constants;
using static Domain.Common.Constants.DomainConstants.City;
using Microsoft.Extensions.Options;
using Application.Services;

namespace Application.Commands.Cities.Validators;

internal sealed class CreateCityCommandValidator : AbstractValidator<CreateCityCommand>
{
    public CreateCityCommandValidator(IErrorLocalizer errors, IOptions<FileUploadSettings> options)
    {
        var opts = options.Value;

        RuleFor(x => x.File).NotNull();
        
        RuleFor(x => x.File.OriginalFileName)
            .NotEmpty()
            .WithMessage(errors[CityErrors.ImageUrlRequired.Code])
            .Must(name => opts.AllowedExtensions
                .Contains(Path.GetExtension(name).ToLowerInvariant()))
            .WithMessage("File type is not allowed.");

        RuleFor(x => x.File.Length)
            .GreaterThan(0)
            .LessThanOrEqualTo(DomainConstants.File.MaxFileSizeBytes);
        
        RuleFor(x => x)
            .Must(x =>
            {
                var ext = Path.GetExtension(x.File.OriginalFileName);
                bool valid = FileSignatureValidator.IsValid(x.File.Stream, ext);
                x.File.Stream.Position = 0; // Reset stream position after validation
                return valid;
            });
        
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