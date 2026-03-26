using Application.Commands.Attractions.Images;
using Domain.Entities.AttractionAggregate;
using FluentValidation;
using static Domain.Common.Constants.DomainConstants.Attraction;

namespace Application.Commands.Attractions.Validators;

internal class AddAttractionImagesCommandValidator : AbstractValidator<AddAttractionImagesCommand>
{
    public AddAttractionImagesCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(AttractionErrors.IdRequired.Message);
        
        RuleFor(x => x.Images)
            .NotEmpty();

        RuleForEach(x => x.Images)
            .SetValidator(new AddAttractionImageDtoValidator());
    }
}

internal class AddAttractionImageDtoValidator : AbstractValidator<AddAttractionImageDto>
{
    public AddAttractionImageDtoValidator()
    {
        RuleFor(x => x.ImageUrl)
            .NotEmpty()
            .WithMessage(AttractionErrors.ImageUrlRequired.Message);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage(AttractionErrors.NegativeDisplayOrder.Message);

        RuleFor(x => x.Caption)
            .Must(caption => caption == null || !string.IsNullOrWhiteSpace(caption))
            .WithMessage(AttractionErrors.CaptionRequired.Message)
            .MaximumLength(MaxDescriptionLength)
            .When(x => x.Caption is not null);
    }
}

internal class RemoveAttractionImagesCommandValidator : AbstractValidator<RemoveAttractionImagesCommand>
{
    public RemoveAttractionImagesCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(AttractionErrors.IdRequired.Message);
        
        RuleFor(x => x.ImageIds)
            .NotEmpty()
            .WithMessage(AttractionErrors.ImageNotFound.Message);
        
        RuleForEach(x => x.ImageIds)
            .NotEmpty()
            .WithMessage(AttractionErrors.ImageNotFound.Message);
    }
}