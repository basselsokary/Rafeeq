using Domain.Entities.AttractionAggregate;
using FluentValidation;
using Application.Common.Interfaces.Localization;

namespace Application.Queries.Attractions.Validators;

internal sealed class GetAttractionByIdForAdminQueryValidator : AbstractValidator<GetAttractionByIdForAdminQuery>
{
    public GetAttractionByIdForAdminQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[AttractionErrors.IdRequired.Code]);
    }
}