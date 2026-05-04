using Domain.Entities.ContentReportAggregate;
using FluentValidation;
using Domain.Common;
using Application.Common.Interfaces.Localization;
using static Domain.Common.Constants.DomainConstants.ContentReport;

namespace Application.Commands.ContentReports.Validators;

internal sealed class ResolveContentReportCommandValidator : AbstractValidator<ResolveContentReportCommand>
{
    public ResolveContentReportCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[ContentReportErrors.IdRequired.Code]);
        
        RuleFor(x => x.Action)
            .IsInEnum()
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code, x.Action.ToString()));
        
        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage(errors[ContentReportErrors.ReasonRequired.Code])
            .MaximumLength(MaxReasonLength)
            .WithMessage(errors.Format(ContentReportErrors.ExceededReasonLength.Code, MaxReasonLength));
    }
}
