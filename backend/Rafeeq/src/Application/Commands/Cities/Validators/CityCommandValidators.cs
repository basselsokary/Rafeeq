using Domain.Entities.CityAggregate;
using FluentValidation;
using static Domain.Common.Constants.DomainConstants.City;

namespace Application.Commands.Cities.Validators;

internal class CreateCityCommandValidator : AbstractValidator<CreateCityCommand>
{
    public CreateCityCommandValidator()
    {
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

internal class DeleteCityCommandValidator : AbstractValidator<DeleteCityCommand>
{
    public DeleteCityCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(CityErrors.IdRequired.Message);
    }
}

internal class UpdateCityCommandValidator : AbstractValidator<UpdateCityCommand>
{
    public UpdateCityCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(CityErrors.IdRequired.Message);

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

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage(CityErrors.NegativeDisplayOrder.Message);
    }
}

