using Domain.Entities.SiteAggregate;
using FluentValidation;
using Domain.Common;
using Application.Common.Interfaces.Localization;
using Domain.ValueObjects;

namespace Application.Commands.Sites.OpeningHours.Validators;

internal sealed class AddSiteOpeningHoursCommandValidator : AbstractValidator<AddSiteOpeningHoursCommand>
{
    public AddSiteOpeningHoursCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[SiteErrors.IdRequired.Code]);
        
        RuleFor(x => x.OpeningHours)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.CollectionRequired.Code]);

        RuleForEach(x => x.OpeningHours)
            .SetValidator(new AddSiteOpeningHoursDtoCommandValidator(errors));
    }
}

internal sealed class AddSiteOpeningHoursDtoCommandValidator : AbstractValidator<AddSiteOpeningHoursDtoCommand>
{
    public AddSiteOpeningHoursDtoCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Day)
            .IsInEnum()
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code, x.Day.ToString()));

        RuleFor(x => x)
            .Must(x => x.StartTime < x.EndTime)
            .WithMessage(errors[TimeRangeErrors.StartTimeNotBeforeEndTime.Code]);
    }
}

