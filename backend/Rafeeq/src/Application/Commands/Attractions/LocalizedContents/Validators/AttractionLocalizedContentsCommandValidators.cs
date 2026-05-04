using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Entities.AttractionAggregate;
using FluentValidation;
using static Domain.Common.Constants.DomainConstants.Attraction;

namespace Application.Commands.Attractions.LocalizedContents.Validators;

internal sealed class AddAttractionLocalizedContentCommandValidator : AbstractValidator<AddAttractionLocalizedContentsCommand>
{
    public AddAttractionLocalizedContentCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[AttractionErrors.IdRequired.Code]);
        
        RuleFor(x => x.LocalizedContents)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.CollectionRequired.Code]);

        RuleForEach(x => x.LocalizedContents)
            .SetValidator(new AddAttractionLocalizedContentsDtoCommandValidator(errors));
    }
}

internal sealed class AddAttractionLocalizedContentsDtoCommandValidator : AbstractValidator<AddAttractionLocalizedContentsDtoCommand>
{
    public AddAttractionLocalizedContentsDtoCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Language)
            .IsInEnum()
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code, x.Language.ToString()));

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(errors[AttractionErrors.NameRequired.Code])
            .MaximumLength(MaxNameLength)
            .WithMessage(errors[AttractionErrors.ExceededNameLength.Code]);

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(errors[AttractionErrors.DescriptionRequired.Code])
            .MaximumLength(MaxDescriptionLength)
            .WithMessage(errors[AttractionErrors.ExceededDescriptionLength.Code]);

        RuleFor(x => x.LocationDescription)
            .MaximumLength(MaxLocationDescriptionLength)
            .When(x => !string.IsNullOrEmpty(x.LocationDescription))
            .WithMessage(errors.Format(AttractionErrors.ExceededLocationDescriptionLength.Code, MaxLocationDescriptionLength));
    }
}
