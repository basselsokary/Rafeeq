using Domain.Entities.ContentReportAggregate;
using FluentValidation;
using Application.Common.Interfaces.Localization;

namespace Application.Commands.ContentReports.Validators;

internal sealed class UnderReviewContentReportCommandValidator : AbstractValidator<UnderReviewContentReportCommand>
{
    public UnderReviewContentReportCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[ContentReportErrors.IdRequired.Code]);
    }
}
