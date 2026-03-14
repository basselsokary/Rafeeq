using Application.Commands.Cities.LocalizedContents;
using Domain.Entities.CityAggregate;
using FluentValidation;

namespace Application.Commands.Cities.Validators;

internal class AddCityLocalizedContentCommandValidator : AbstractValidator<AddCityLocalizedContentCommand>
{
    public AddCityLocalizedContentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(CityErrors.IdRequired.Message);
        
        RuleFor(x => x.Language)
            .IsInEnum();
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(CityErrors.NameRequired.Message);
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(CityErrors.DescriptionRequired.Message);
    }
}
