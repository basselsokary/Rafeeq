using Domain.Entities.AttractionAggregate;
using FluentValidation;

namespace Application.Commands.Attractions.Validators;

internal class AddAttractionLocalizedContentCommandValidator : AbstractValidator<AddAttractionLocalizedContentCommand>
{
    public AddAttractionLocalizedContentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(AttractionErrors.IdRequired.Message);
        
        RuleFor(x => x.Language)
            .IsInEnum();
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(AttractionErrors.NameRequired.Message);
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(AttractionErrors.DescriptionRequired.Message);
    }
}
