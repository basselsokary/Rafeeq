using FluentValidation;
using Application.Queries.Common.Validators;
using Domain.Common.Constants;
using Application.Common.Interfaces.Localization;
using Domain.Common;

namespace Application.Queries.ContentReports.Validators;

internal sealed class GetContentReportsByHighPriorityQueryValidator : AbstractValidator<GetContentReportsByHighPriorityQuery>
{
    public GetContentReportsByHighPriorityQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Priority)
            .GreaterThanOrEqualTo(1)
            .WithMessage(errors[ValidationErrors.NumberMustBeGreaterThanZero.Code])
            .LessThanOrEqualTo(DomainConstants.ContentReport.HighPriority)
            .WithMessage(errors[ValidationErrors.MaximumLengthExceeded.Code]);

        RuleFor(x => x.Paging)
            .NotNull()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code])
            .SetValidator(new PagingParametersValidator(errors));

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue)
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code, x.Status.ToString() ?? "null"));

        RuleFor(x => x.Reason)
            .IsInEnum()
            .When(x => x.Reason.HasValue)
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code, x.Reason.ToString() ?? "null"));
    }
}