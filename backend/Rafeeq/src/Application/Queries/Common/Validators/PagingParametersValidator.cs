using Application.DTOs.Common;
using FluentValidation;
using Application.Common.Interfaces.Localization;
using Domain.Common;

namespace Application.Queries.Common.Validators;

internal sealed class PagingParametersValidator : AbstractValidator<PagingParameters>
{
    public PagingParametersValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage(errors[ValidationErrors.NumberMustBeGreaterThanZero.Code]);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage(errors[ValidationErrors.NumberMustBeGreaterThanZero.Code])
            .LessThanOrEqualTo(100)
            .WithMessage(errors[ValidationErrors.MaximumLengthExceeded.Code]);
    }
}
