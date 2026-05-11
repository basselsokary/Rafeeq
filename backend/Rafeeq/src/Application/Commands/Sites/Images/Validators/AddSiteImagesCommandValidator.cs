using Application.Common.Interfaces.Localization;
using Application.Common.Validators;
using Application.Services;
using Domain.Common;
using Domain.Common.Constants;
using Domain.Entities.SiteAggregate;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Application.Commands.Sites.Images.Validators;

internal sealed class AddSiteImagesCommandValidator : AbstractValidator<AddSiteImagesCommand>
{
    public AddSiteImagesCommandValidator(IErrorLocalizer errors, IOptions<FileUploadSettings> options)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[SiteErrors.IdRequired.Code]);
        
        RuleFor(x => x.Images)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.CollectionRequired.Code])
            .Must(x => x.Count <= DomainConstants.File.MaxImagesPerRequest);

        RuleForEach(x => x.Images)
            .SetValidator(new AddSiteImageDtoValidator(errors, options));
    }
}

internal sealed class AddSiteImageDtoValidator : AbstractValidator<AddSiteImageDto>
{
    public AddSiteImageDtoValidator(IErrorLocalizer errors, IOptions<FileUploadSettings> options)
    {
        var opts = options.Value;

        RuleFor(x => x.File).NotNull();
        
        RuleFor(x => x.File.OriginalFileName)
            .NotEmpty()
            .WithMessage(errors[SiteErrors.ImageUrlRequired.Code])
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

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage(errors[SiteErrors.NegativeDisplayOrder.Code]);
        
        RuleFor(x => x.Caption)
            .MaximumLength(DomainConstants.File.MaxCaptionLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Caption))
            .WithMessage(errors[SiteErrors.CaptionRequired.Code]);
    }
}