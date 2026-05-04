using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Entities.AttractionAggregate;
using FluentValidation;
using static Domain.Common.Constants.DomainConstants.Attraction;

namespace Application.Commands.Attractions.LocalizedContents.Validators;

internal sealed class UpdateAttractionLocalizedContentCommandValidator : AbstractValidator<UpdateAttractionLocalizedContentCommand>
{
    public UpdateAttractionLocalizedContentCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[AttractionErrors.IdRequired.Code]);

        RuleFor(x => x.LocalizedContents)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.CollectionRequired.Code]);

        RuleForEach(x => x.LocalizedContents)
            .SetValidator(new UpdateAttractionLocalizedContentsDtoValidator(errors));
    }
}

internal class UpdateAttractionLocalizedContentsDtoValidator : AbstractValidator<UpdateAttractionLocalizedContentsDtoCommand>
{
    public UpdateAttractionLocalizedContentsDtoValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Language)
            .IsInEnum()
            .WithMessage(errors[ValidationErrors.InvalidEnumValue.Code]);

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(errors[AttractionErrors.NameRequired.Code])
            .MaximumLength(MaxNameLength)
            .WithMessage(errors.Format(AttractionErrors.ExceededNameLength.Code, MaxNameLength));
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(errors[AttractionErrors.DescriptionRequired.Code])
            .MaximumLength(MaxDescriptionLength)
            .WithMessage(errors.Format(AttractionErrors.ExceededDescriptionLength.Code, MaxDescriptionLength));
        
        RuleFor(x => x.LocationDescription)
            .MaximumLength(MaxLocationDescriptionLength)
            .When(x => !string.IsNullOrEmpty(x.LocationDescription))
            .WithMessage(errors.Format(AttractionErrors.ExceededLocationDescriptionLength.Code, MaxLocationDescriptionLength));
    }
}