using Domain.Entities.CityAggregate;
using FluentValidation;
using Application.Common.Interfaces.Localization;

namespace Application.Queries.Cities.LocalizedContents.Validators;

internal sealed class GetCityLocalizedContentsQueryValidator : AbstractValidator<GetCityLocalizedContentsQuery>
{
    public GetCityLocalizedContentsQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.CityId)
            .NotEmpty()
            .WithMessage(errors[CityErrors.IdRequired.Code]);
    }
}