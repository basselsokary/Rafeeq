using Application.Common.Interfaces.Localization;
using Application.Common.Validators;
using Domain.Common;
using Domain.Common.Constants;
using Domain.Entities.AttractionAggregate;
using FluentValidation;

namespace Application.Commands.Attractions.Images.Validators;

internal sealed class AddAttractionImagesCommandValidator : AbstractValidator<AddAttractionImagesCommand>
{
    public AddAttractionImagesCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[AttractionErrors.IdRequired.Code]);
        
        RuleFor(x => x.Images)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.CollectionRequired.Code])
            .Must(x => x.Count <= DomainConstants.Image.MaxImagesPerRequest);

        RuleForEach(x => x.Images)
            .SetValidator(new AddAttractionImageDtoValidator(errors));
    }
}

internal sealed class AddAttractionImageDtoValidator : AbstractValidator<AddAttractionImageDto>
{
    public AddAttractionImageDtoValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.OriginalFileName)
            .NotEmpty()
            .WithMessage(errors[AttractionErrors.ImageUrlRequired.Code])
            .MaximumLength(DomainConstants.Image.MaxImageUrlLength);

        RuleFor(x => x.Stream.Length)
            .GreaterThan(0)
            .LessThanOrEqualTo(DomainConstants.Image.MaxFileSizeBytes);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage(errors[AttractionErrors.NegativeDisplayOrder.Code]);

        RuleFor(x => x.Caption)
            .MaximumLength(DomainConstants.Image.MaxCaptionLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Caption))
            .WithMessage(errors.Format(AttractionErrors.ExceededImageCaptionLength.Code, DomainConstants.Image.MaxCaptionLength));
    
        RuleFor(x => x)
            .Must(x =>
            {
                var ext = Path.GetExtension(x.OriginalFileName);
                return FileSignatureValidator.IsValid(x.Stream, ext);
            });
    }
}
