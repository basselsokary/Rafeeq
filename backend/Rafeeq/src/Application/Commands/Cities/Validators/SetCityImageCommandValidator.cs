using Domain.Entities.CityAggregate;
using FluentValidation;
using Application.Common.Interfaces.Localization;
using Application.Common.Validators;
using Domain.Common.Constants;
using Microsoft.Extensions.Options;
using Application.Services;

namespace Application.Commands.Cities.Validators;

internal sealed class SetCityImageCommandValidator : AbstractValidator<SetCityImageCommand>
{
    public SetCityImageCommandValidator(IErrorLocalizer errors, IOptions<FileUploadSettings> options)
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

        RuleFor(x => x.CityId)
            .NotEmpty()
            .WithMessage(errors[CityErrors.IdRequired.Code]);
    }
}
