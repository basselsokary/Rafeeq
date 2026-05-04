using Domain.Entities.ContentReportAggregate;
using FluentValidation;
using Application.Common.Interfaces.Localization;

namespace Application.Commands.ContentReports.Validators;

internal sealed class EscalateContentReportCommandValidator : AbstractValidator<EscalateContentReportCommand>
{
    public EscalateContentReportCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[ContentReportErrors.IdRequired.Code]);
    }
}
