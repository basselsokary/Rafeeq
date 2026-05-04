using FluentValidation;
using Domain.Entities.CityAggregate;
using Application.Common.Interfaces.Localization;

namespace Application.Queries.Cities.Validators;

internal sealed class GetCityByIdQueryValidator : AbstractValidator<GetCityByIdQuery>
{
    public GetCityByIdQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[CityErrors.IdRequired.Code]);
    }
}
