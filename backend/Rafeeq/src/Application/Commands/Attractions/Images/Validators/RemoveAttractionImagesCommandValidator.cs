using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Entities.AttractionAggregate;
using FluentValidation;

namespace Application.Commands.Attractions.Images.Validators;

internal sealed class RemoveAttractionImagesCommandValidator : AbstractValidator<RemoveAttractionImagesCommand>
{
    public RemoveAttractionImagesCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[AttractionErrors.IdRequired.Code]);
        
        RuleFor(x => x.ImageIds)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.CollectionRequired.Code]);
        
        RuleForEach(x => x.ImageIds)
            .NotEmpty()
            .WithMessage(errors[ImageErrors.ImageIdRequired.Code]);
    }
}