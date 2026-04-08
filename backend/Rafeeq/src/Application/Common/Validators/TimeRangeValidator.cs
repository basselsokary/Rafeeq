using Domain.ValueObjects;
using FluentValidation;

namespace Application.Common.Validators;

internal class TimeRangeValidator : AbstractValidator<TimeRange>
{
    public TimeRangeValidator()
    {
        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime);
    }
}
