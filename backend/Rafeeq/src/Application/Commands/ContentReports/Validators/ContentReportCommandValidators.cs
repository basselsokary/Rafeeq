using Domain.Entities.ContentReportAggregate;
using FluentValidation;
using static Domain.Common.Constants.DomainConstants.ContentReport;

namespace Application.Commands.ContentReports.Validators;

internal class EscalateContentReportCommandValidator : AbstractValidator<EscalateContentReportCommand>
{
    public EscalateContentReportCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(ContentReportErrors.IdRequired.Message);
    }
}

internal class ReportContentCommandValidator : AbstractValidator<ReportContentCommand>
{
    public ReportContentCommandValidator()
    {
        RuleFor(x => x.ReportedEntityId)
            .NotEmpty()
            .WithMessage(ContentReportErrors.ReportedEntityIdRequired.Message);

        RuleFor(x => x.Reason)
            .IsInEnum();

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(ContentReportErrors.DescriptionRequired.Message)
            .MaximumLength(MaxDescriptionLength)
            .WithMessage(ContentReportErrors.ExceededDescriptionLength.Message);
    }
}

internal class ResolveContentReportCommandValidator : AbstractValidator<ResolveContentReportCommand>
{
    public ResolveContentReportCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(ContentReportErrors.IdRequired.Message);
        
        RuleFor(x => x)
            .Must(x => (x.Reason is null) ^ (x.Action is null))
            .WithMessage(ContentReportErrors.CannotBeResolved.Message);
    }
}

internal class UnderReviewContentReportCommandValidator : AbstractValidator<UnderReviewContentReportCommand>
{
    public UnderReviewContentReportCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(ContentReportErrors.IdRequired.Message);
    }
}
