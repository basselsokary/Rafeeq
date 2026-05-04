using Application.Common.Interfaces.Localization;
using Application.Common.Validators;
using Domain.Common;
using Domain.Common.Constants;
using Domain.Entities.SponsorAggregate;
using FluentValidation;

namespace Application.Commands.Sponsors.Images.Validators;

internal sealed class AddSponsorImagesCommandValidator : AbstractValidator<AddSponsorImagesCommand>
{
    public AddSponsorImagesCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.IdRequired.Code]);
        
        RuleFor(x => x.Images)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.CollectionRequired.Code])
            .Must(x => x.Count <= DomainConstants.Image.MaxImagesPerRequest);

        RuleForEach(x => x.Images)
            .SetValidator(new AddSponsorImageDtoValidator(errors));
    }
}

internal sealed class AddSponsorImageDtoValidator : AbstractValidator<AddSponsorImageDto>
{
    public AddSponsorImageDtoValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.OriginalFileName)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.ImageUrlRequired.Code])
            .MaximumLength(DomainConstants.Image.MaxImageUrlLength);

        RuleFor(x => x.Stream.Length)
            .GreaterThan(0)
            .LessThanOrEqualTo(DomainConstants.Image.MaxFileSizeBytes);
        
        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage(errors[SponsorErrors.NegativeDisplayOrder.Code]);

        RuleFor(x => x.Caption)
            .MaximumLength(DomainConstants.Image.MaxCaptionLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Caption))
            .WithMessage(errors[SponsorErrors.ExceededImageLength.Code]);
        
        RuleFor(x => x)
            .Must(x =>
            {
                var ext = Path.GetExtension(x.OriginalFileName);
                return FileSignatureValidator.IsValid(x.Stream, ext);
            });    
    }
}

