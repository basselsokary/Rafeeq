using Domain.Entities.AttractionAggregate;
using FluentValidation;
using Application.Queries.Common.Validators;
using Application.Common.Interfaces.Localization;
using Domain.Common;

namespace Application.Queries.Attractions.Validators;

internal sealed class GetAttractionsBySiteIdQueryValidator : AbstractValidator<GetAttractionsBySiteIdQuery>
{
    public GetAttractionsBySiteIdQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.SiteId)
            .NotEmpty()
            .WithMessage(errors[AttractionErrors.SiteIdRequired.Code]);

        RuleFor(x => x.Type)
            .IsInEnum()
            .When(x => x.Type != null)
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code, x.Type?.ToString()!));

        RuleFor(x => x.SearchTerm)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.SearchTerm))
            .WithMessage(errors.Format(ValidationErrors.MaximumLengthExceeded.Code));
    }
}
