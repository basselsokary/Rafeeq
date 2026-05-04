using Domain.Entities.AttractionAggregate;
using FluentValidation;
using Application.Common.Interfaces.Localization;

namespace Application.Queries.Attractions.LocalizedContents.Validators;

internal sealed class GetAttractionLocalizedContentsQueryValidator : AbstractValidator<GetAttractionLocalizedContentsQuery>
{
    public GetAttractionLocalizedContentsQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.AttractionId)
            .NotEmpty()
            .WithMessage(errors[AttractionErrors.IdRequired.Code]);
    }
}
