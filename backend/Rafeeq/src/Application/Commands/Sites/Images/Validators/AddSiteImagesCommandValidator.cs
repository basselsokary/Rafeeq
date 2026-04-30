using Application.Common.Interfaces.Localization;
using Application.Common.Validators;
using Domain.Common;
using Domain.Common.Constants;
using Domain.Entities.SiteAggregate;
using FluentValidation;

namespace Application.Commands.Sites.Images.Validators;

internal sealed class AddSiteImagesCommandValidator : AbstractValidator<AddSiteImagesCommand>
{
    public AddSiteImagesCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[SiteErrors.IdRequired.Code]);
        
        RuleFor(x => x.Images)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.CollectionRequired.Code])
            .Must(x => x.Count <= DomainConstants.Image.MaxImagesPerRequest);

        RuleForEach(x => x.Images)
            .SetValidator(new AddSiteImageDtoValidator(errors));
    }
}

internal sealed class AddSiteImageDtoValidator : AbstractValidator<AddSiteImageDto>
{
    public AddSiteImageDtoValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.OriginalFileName)
            .NotEmpty()
            .WithMessage(errors[SiteErrors.ImageUrlRequired.Code])
            .MaximumLength(DomainConstants.Image.MaxImageUrlLength);

        RuleFor(x => x.Stream.Length)
            .GreaterThan(0)
            .LessThanOrEqualTo(DomainConstants.Image.MaxFileSizeBytes);
        
        RuleFor(x => x)
            .Must(x =>
            {
                var ext = Path.GetExtension(x.OriginalFileName);
                return FileSignatureValidator.IsValid(x.Stream, ext);
            });

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage(errors[SiteErrors.NegativeDisplayOrder.Code]);
        
        RuleFor(x => x.Caption)
            .MaximumLength(DomainConstants.Image.MaxCaptionLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Caption))
            .WithMessage(errors[SiteErrors.ExceededImageLength.Code]);        
    }
}

