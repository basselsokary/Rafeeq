using Application.Commands.Sites.OpeningHours;
using Domain.Entities.SiteAggregate;
using FluentValidation;

namespace Application.Commands.Sites.Validators;

internal class AddSiteOpeningHoursCommandValidator : AbstractValidator<AddSiteOpeningHoursCommand>
{
    public AddSiteOpeningHoursCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SiteErrors.IdRequired.Message);
        
        RuleFor(x => x.OpeningTime)
            .NotNull();
        
        RuleFor(x => x.DayOfWeek)
            .IsInEnum();
    }
}

