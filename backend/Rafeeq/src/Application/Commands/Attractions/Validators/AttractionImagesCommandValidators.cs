using Domain.Entities.AttractionAggregate;
using FluentValidation;

namespace Application.Commands.Attractions.Validators;

internal class AddAttractionImagesCommandValidator : AbstractValidator<AddAttractionImagesCommand>
{
    public AddAttractionImagesCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(AttractionErrors.IdRequired.Message);
        
        RuleFor(x => x.ImageUrl)
            .NotEmpty()
            .WithMessage(AttractionErrors.ImageUrlRequired.Message);
    }
}

internal class RemoveAttractionImagesCommandValidator : AbstractValidator<RemoveAttractionImagesCommand>
{
    public RemoveAttractionImagesCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(AttractionErrors.IdRequired.Message);
        
        RuleFor(x => x.ImageId)
            .NotEmpty()
            .WithMessage(AttractionErrors.ImageUrlRequired.Message);
    }
}