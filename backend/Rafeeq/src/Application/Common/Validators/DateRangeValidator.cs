using Domain.ValueObjects;
using FluentValidation;

namespace Application.Common.Validators;

internal class DateRangeValidator : AbstractValidator<DateRange>
{
    public DateRangeValidator()
    {
        RuleFor(x => x.StartDate)
            .LessThan(x => x.EndDate);
    }
}
