using Application.Commands.Cities.LocalizedContents;
using Domain.Entities.CityAggregate;
using FluentValidation;
using static Domain.Common.Constants.DomainConstants.City;

namespace Application.Commands.Cities.Validators;

internal class AddCityLocalizedContentCommandValidator : AbstractValidator<AddCityLocalizedContentsCommand>
{
    public AddCityLocalizedContentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(CityErrors.IdRequired.Message);
        
        RuleFor(x => x.LocalizedContents)
            .NotEmpty();

        RuleForEach(x => x.LocalizedContents)
            .SetValidator(new AddCityLocalizedContentsDtoCommandValidator());
    }
}

internal class AddCityLocalizedContentsDtoCommandValidator : AbstractValidator<AddCityLocalizedContentsDtoCommand>
{
    public AddCityLocalizedContentsDtoCommandValidator()
    {
        RuleFor(x => x.Language)
            .IsInEnum();

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(CityErrors.NameRequired.Message)
            .MaximumLength(MaxNameLength)
            .WithMessage(CityErrors.ExceededNameLength.Message);

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(CityErrors.DescriptionRequired.Message)
            .MaximumLength(MaxDescriptionLength)
            .WithMessage(CityErrors.ExceededDescriptionLength.Message);
    }
}
