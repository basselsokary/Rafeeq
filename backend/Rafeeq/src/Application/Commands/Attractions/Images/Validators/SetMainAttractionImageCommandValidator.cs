using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Entities.AttractionAggregate;
using FluentValidation;

namespace Application.Commands.Attractions.Images.Validators;

internal sealed class SetMainAttractionImageCommandValidator : AbstractValidator<SetMainAttractionImageCommand>
{
    public SetMainAttractionImageCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[AttractionErrors.IdRequired.Code]);
        
        RuleFor(x => x.ImageId)
            .NotEmpty()
            .WithMessage(errors[ImageErrors.ImageIdRequired.Code]);
    }
}

