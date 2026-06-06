using Application.Common.Interfaces.Localization;
using Application.Common.Validators;
using Application.Services;
using Domain.Common;
using Domain.Common.Constants;
using Domain.Entities.SponsorAggregate;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Application.Commands.Sponsors.Images.Validators;

internal sealed class AddSponsorImagesCommandValidator : AbstractValidator<AddSponsorImagesCommand>
{
    public AddSponsorImagesCommandValidator(IErrorLocalizer errors, IOptions<FileUploadOptions> options, FileSignatureValidator fileSignatureValidator)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.IdRequired.Code]);
        
        RuleFor(x => x.Images)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.CollectionRequired.Code])
            .Must(x => x.Count <= DomainConstants.File.MaxImagesPerRequest)
            .WithMessage(x => $"Maximum allowed images is {DomainConstants.File.MaxImagesPerRequest}. Provided {x.Images.Count} count.");

        RuleForEach(x => x.Images)
            .SetValidator(new AddSponsorImageDtoValidator(errors, options, fileSignatureValidator));
    }
}

internal sealed class AddSponsorImageDtoValidator : AbstractValidator<AddSponsorImageDto>
{
    public AddSponsorImageDtoValidator(IErrorLocalizer errors, IOptions<FileUploadOptions> options, FileSignatureValidator fileSignatureValidator)
    {
        var opts = options.Value;

        RuleFor(x => x.File)
            .NotNull()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code]);
        
        RuleFor(x => x.File.OriginalFileName)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.ImageUrlRequired.Code])
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
                bool valid = fileSignatureValidator.IsValid(x.File.Stream, ext);
                x.File.Stream.Position = 0; // Reset stream position after validation
                return valid;
            })
            .WithMessage("File content does not match the file extension.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage(errors[SponsorErrors.NegativeDisplayOrder.Code]);
        
        RuleFor(x => x.Caption)
            .MaximumLength(DomainConstants.File.MaxCaptionLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Caption))
            .WithMessage(errors[SponsorErrors.CaptionRequired.Code]);    
    }
}

