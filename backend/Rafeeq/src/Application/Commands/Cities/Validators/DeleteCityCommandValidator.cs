using Domain.Entities.CityAggregate;
using FluentValidation;
using Application.Common.Interfaces.Localization;

namespace Application.Commands.Cities.Validators;

internal sealed class DeleteCityCommandValidator : AbstractValidator<DeleteCityCommand>
{
    public DeleteCityCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[CityErrors.IdRequired.Code]);
    }
}

