using Application.Commands.Sites.OpeningHours;
using Domain.Entities.SiteAggregate;
using FluentValidation;
using Application.Common.Validators;

namespace Application.Commands.Sites.Validators;

internal class AddSiteOpeningHoursCommandValidator : AbstractValidator<AddSiteOpeningHoursCommand>
{
    public AddSiteOpeningHoursCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SiteErrors.IdRequired.Message);
        
        RuleFor(x => x.OpeningHours)
            .NotEmpty();

        RuleForEach(x => x.OpeningHours)
            .SetValidator(new AddSiteOpeningHoursDtoCommandValidator());
    }
}

internal class AddSiteOpeningHoursDtoCommandValidator : AbstractValidator<AddSiteOpeningHoursDtoCommand>
{
    public AddSiteOpeningHoursDtoCommandValidator()
    {
        RuleFor(x => x.DayOfWeek)
            .IsInEnum();

        RuleFor(x => x.OpeningTime)
            .NotNull()
            .SetValidator(new TimeRangeValidator());
    }
}

