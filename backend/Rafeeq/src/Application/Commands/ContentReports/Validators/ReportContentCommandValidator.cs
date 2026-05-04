using Domain.Entities.ContentReportAggregate;
using FluentValidation;
using Domain.Common;
using Application.Common.Interfaces.Localization;
using static Domain.Common.Constants.DomainConstants.ContentReport;

namespace Application.Commands.ContentReports.Validators;

internal sealed class ReportContentCommandValidator : AbstractValidator<ReportContentCommand>
{
    public ReportContentCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.ReportedEntityId)
            .NotEmpty()
            .WithMessage(errors[ContentReportErrors.ReportedEntityIdRequired.Code]);

        RuleFor(x => x.Reason)
            .IsInEnum()
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code, x.Reason.ToString()));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(errors[ContentReportErrors.DescriptionRequired.Code])
            .MaximumLength(MaxDescriptionLength)
            .WithMessage(errors.Format(ContentReportErrors.ExceededDescriptionLength.Code, MaxDescriptionLength));
    }
}
