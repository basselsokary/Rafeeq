using Application.Common.Interfaces.Localization;
using Domain.Entities.AttractionAggregate;
using FluentValidation;

namespace Application.Queries.Attractions.LocalizedContents.Validators;

internal sealed class GetAttractionLocalizedContentByIdQueryValidator : AbstractValidator<GetAttractionLocalizedContentByIdQuery>
{
    public GetAttractionLocalizedContentByIdQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.AttractionId)
            .NotEmpty()
            .WithMessage(errors[AttractionErrors.IdRequired.Code]);

        RuleFor(x => x.ContentId)
            .NotEmpty()
            .WithMessage(errors[AttractionErrors.ContentIdRequired.Code]);
    }
}
