using Domain.Entities.SiteAggregate;
using FluentValidation;
using Domain.Common;
using Application.Common.Interfaces.Localization;

namespace Application.Commands.Sites.OpeningHours.Validators;

internal sealed class RemoveSiteOpeningHoursCommandValidator : AbstractValidator<RemoveSiteOpeningHoursCommand>
{
    public RemoveSiteOpeningHoursCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[SiteErrors.IdRequired.Code]);
        
        RuleFor(x => x.Days)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.CollectionRequired.Code]);

        RuleForEach(x => x.Days)
            .IsInEnum()
            .WithMessage(errors[ValidationErrors.InvalidEnumValue.Code]);
    }
}

